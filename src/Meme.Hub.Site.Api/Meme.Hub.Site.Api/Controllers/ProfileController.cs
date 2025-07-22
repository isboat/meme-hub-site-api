using Meme.Hub.Site.Api.Models;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Models.ProfileModels;
using Meme.Hub.Site.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meme.Hub.Site.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : CustomBaseController
    {
        private readonly IProfileService _profileService;
        private readonly IStorageService _storageService;

        private const long UploadMaxSixe = 3_000_000_000;
        private const string allowedImageFileExt = "image/jpeg,image/png,image/gif";

        public ProfileController(IProfileService profileService, IStorageService storageService)
        {
            _profileService = profileService;
            _storageService = storageService;
        }

        // POST: /api/auth/gettoken
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfile>> GetProfile(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _profileService.GetProfile(id);
                if (response == null)
                {
                    return NotFound();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getting profile by id: {ex.Message}");
                return StatusCode(500, new { message = "An internal server error occurred" });
            }
        }

        // POST: /api/auth/gettoken
        [HttpGet("kols")]
        public async Task<ActionResult<List<UserProfile>>> GetKolProfiles()
        {
            try
            {
                var response = await _profileService.GetKolsProfile();
                if (response == null)
                {
                    return NotFound();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getting kols: {ex.Message}");
                return StatusCode(500, new { message = "An internal server error occurred" });
            }
        }

        // POST: /api/auth/refresh_token
        [HttpPost("create")]
        [Authorize()]
        public async Task<ActionResult<AuthResponseDto>> Create([FromBody] UserProfile request)
        {
            var userId = GetRequestUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "Refresh token is required." });
            }

            request.UserId = userId;
            request.Id = userId;

            try
            {
                var response = await _profileService.CreateProfile(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating profile: {ex.Message}");
                return StatusCode(500, new { message = "An internal server error occurred while creating profile." });
            }
        }

        // POST: /api/auth/refresh_token
        [HttpPost("{id}/follow")]
        [Authorize()]
        public async Task<ActionResult<AuthResponseDto>> Follow(string id)
        {
            var followerId = GetRequestUserId();
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(followerId))
            {
                return BadRequest(new { message = "ID or followerId is required." });
            }

            try
            {
                await _profileService.AddFollower(id, followerId);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding Follower: {ex.Message}");
                return StatusCode(500, new { message = "An internal server error occurred while adding Follower." });
            }
        }

        // POST: /api/auth/refresh_token
        [HttpPost("{id}/unfollow")]
        [Authorize()]
        public async Task<ActionResult<AuthResponseDto>> UnFollow(string id)
        {
            var followerId = GetRequestUserId();
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(followerId))
            {
                return BadRequest(new { message = "ID or followerId is required." });
            }

            try
            {
                await _profileService.RemoveFollower(id, followerId);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RemoveFollower: {ex.Message}");
                return StatusCode(500, new { message = "An internal server error occurred while RemoveFollower." });
            }
        }

        [HttpPut("update-with-image")]
        [Authorize()]
        public async Task<ActionResult> UpdateProfileRequest([FromForm] UpdateProfileRequestModel model)
        {
            var userId = GetRequestUserId();
            var bannerStoragePath = "";
            long size = 0;
            var fileName = "";

            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                bool isImageFile = allowedImageFileExt.Contains(model.ProfileImageFile.ContentType);
                if (!isImageFile)
                {
                    return BadRequest($"{model.ProfileImageFile.ContentType} Not allowed");
                }

                if(model.ProfileImageFile.Length > UploadMaxSixe)
                {
                    return BadRequest($"Profile image too large");
                }

                size = model.ProfileImageFile.Length;
                if (model.ProfileImageFile.Length > 0)
                {
                    fileName = model.ProfileImageFile.FileName.ToLowerInvariant();
                    await using var stream = model.ProfileImageFile.OpenReadStream();
                    bannerStoragePath = await _storageService.UploadAsync(userId, fileName, stream);
                }
            }

            await _profileService.UpdateProfile(userId, new UserProfile
            {
                Description = model.Bio,
                ProfileName = model.Username,
                ProfileImage = bannerStoragePath,
                Language = model.Language,
                Location = model.Location,
                LastUpdatedAt = DateTime.UtcNow
            });

            return Ok("Form submitted successfully!");
        }
    }
}
