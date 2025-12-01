using Meme.Hub.Site.Api.Models;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Models.ProfileModels;
using Meme.Hub.Site.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meme.Hub.Site.Api.Controllers
{
    [ApiController]
    [Route("api/token-profile")]
    public class TokenProfileController : CustomBaseController
    {
        private readonly ITokenProfileService _profileService;
        private readonly IStorageService _storageService;
        private readonly IDatabaseService _databaseService;
        private readonly ICacheService _cacheService;

        private const long UploadMaxSixe = 3_000_000_000;
        private const string allowedImageFileExt = "image/jpeg,image/png,image/gif";

        public TokenProfileController(ITokenProfileService profileService, IStorageService storageService, IDatabaseService databaseService, ICacheService cacheService)
        {
            _profileService = profileService;
            _storageService = storageService;
            _databaseService = databaseService;
            _cacheService = cacheService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TokenProfile>> GetProfile(string id)
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

        [HttpGet("socials")]
        public async Task<IActionResult> GetSocials()
        {
            var socials = await _databaseService.GetSocialClaims();
            return new OkObjectResult(socials);
        }

        [HttpGet("socials/{tokenAddress}")]
        public async Task<IActionResult> GetSocials(string tokenAddress)
        {
            if (string.IsNullOrWhiteSpace(tokenAddress)) return BadRequest();

            var dbEleme = await _databaseService.GetTokenSocialsClaimByTokenAddress(tokenAddress);

            return new OkObjectResult(dbEleme);
        }

        [HttpPost("submit-socials")]
        [Authorize()]
        public async Task<ActionResult> SubmitSocials([FromForm] SubmitSocialsRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bannerStoragePath = string.Empty;
            var logoStoragePath = string.Empty;

            try
            {
                bannerStoragePath = await UploadTokenImage(model.TokenAddress, model.ProfileImageFile);
                logoStoragePath = await UploadTokenImage(model.TokenAddress, model.ProfileLogoImageFile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var id = _databaseService.SaveSubmitedSocialsToken(new SocialsClaimModel
            {
                UserId = model.UserId,
                Description = model.Description,
                TokenAddress = model.TokenAddress,
                TelegramUsername = model.TelegramUsername,
                Chain = model.Chain,
                Discord = model.Discord,
                DiscordUsername = model.DiscordUsername,
                Others = model.Other,
                Reddit = model.Reddit,                
                Telegram = model.Telegram,
                TokenName = model.TokenName,
                Twitter = model.Twitter,
                Website = model.Website,
                BannerUrl = bannerStoragePath,
                LogoUrl = logoStoragePath,
                TokenData = await _cacheService.GetTokenData(model.TokenAddress),
                Approvers = [],
                SubmitedAt = DateTime.UtcNow,
                Status = SocialsClaimStatus.Pending
            });

            return Ok(new { Id = id});
        }

        private async Task<string> UploadTokenImage(string tokenAddress, IFormFile imageFile)
        {
            var bannerStoragePath = "";
            if (imageFile != null && imageFile.Length > 0)
            {
                bool isImageFile = allowedImageFileExt.Contains(imageFile.ContentType);
                if (!isImageFile)
                {
                    throw new Exception($"{imageFile.ContentType} Not allowed");
                }

                if (imageFile.Length > 0)
                {
                    var fileName = imageFile.FileName.ToLowerInvariant();
                    await using var stream = imageFile.OpenReadStream();
                    bannerStoragePath = await _storageService.UploadAsync(tokenAddress, fileName, stream);
                }
            }

            return bannerStoragePath;
        }

        [HttpGet("user-pending-tokenclaims")]
        [Authorize()]
        public async Task<ActionResult> UserPendingSocialClaims()
        {
            var userId = GetRequestUserId();
            var claims = await _databaseService.GetUserPendingSocialsClaims(userId);

            return Ok(claims);
        }

        [HttpGet("pending-tokenclaims/{claimId}")]
        public async Task<ActionResult> GetPendingSocialClaims(string claimId)
        {
            var userId = GetRequestUserId();
            var claims = await _databaseService.GetTokenSocialsClaimById(claimId);

            return Ok(claims);
        }

        [HttpPost("approve-tokensocialsclaim/{id}")]
        public async Task<ActionResult> ApproveSocials(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var userId = GetRequestUserId();
            var response = await _databaseService.ApproveSubmitedSocialsToken(id, userId);

            return Ok("approved successfully!");
        }
    }
}
