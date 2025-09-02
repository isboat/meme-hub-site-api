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

        [HttpGet("socials/{tokenAddress}")]
        public async Task<IActionResult> GetSocials(string tokenAddress)
        {
            if (string.IsNullOrWhiteSpace(tokenAddress)) return BadRequest();

            var dbEleme = await _databaseService.GetSocialsByAddress(tokenAddress);

            return new OkObjectResult(dbEleme);
        }

        [HttpPost("submit-socials")]
        [Authorize()]
        public async Task<ActionResult> SubmitSocials([FromForm] SubmitSocialsRequestModel model)
        {

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

                size = model.ProfileImageFile.Length;
                if (model.ProfileImageFile.Length > 0)
                {
                    fileName = model.ProfileImageFile.FileName.ToLowerInvariant();
                    await using var stream = model.ProfileImageFile.OpenReadStream();
                    bannerStoragePath = await _storageService.UploadAsync(model.TokenAddress, fileName, stream);
                }
            }

            _ = _databaseService.SaveSubmitedSocialsToken(new SubmitSocialsClaimModel
            {
                UserId = model.UserId,
                Description = model.Description,
                TokenAddress = model.TokenAddress,
                TelegramUsername = model.TelegramUsername,
                Chain = model.Chain,
                Discord = model.Discord,
                DiscordUsername = model.DiscordUsername,
                Id = Guid.NewGuid().ToString("N"),
                Others = model.Other,
                Reddit = model.Reddit,                
                Telegram = model.Telegram,
                TokenName = model.TokenName,
                Twitter = model.Twitter,
                Website = model.Website,
                BannerUrl = bannerStoragePath,
                TokenData = await _cacheService.GetTokenData(model.TokenAddress),
            });

            //_ = _databaseService.ApproveSubmitedSocialsToken(model.TokenAddress);

            return Ok("Form submitted successfully!");
        }

        [HttpGet("approve-socials/{addr}")]
        public async Task<ActionResult> ApproveSocials(string addr)
        {
            _ = _databaseService.ApproveSubmitedSocialsToken(addr);

            return Ok("approved successfully!");
        }
    }
}
