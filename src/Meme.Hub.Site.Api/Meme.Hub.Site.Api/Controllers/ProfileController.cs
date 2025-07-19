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

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
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
    }
}
