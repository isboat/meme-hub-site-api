using Meme.Hub.Site.Api.Models;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace Meme.Hub.Site.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {

        private readonly ILogger<TokenController> _logger;

        private readonly ICacheService _cacheService;
        private readonly IDatabaseService _databaseService;
        private readonly IStorageService _storageService;

        private const long UploadMaxSixe = 3_000_000_000;
        private const string allowedImageFileExt = "image/jpeg,image/png,image/gif";

        public TokenController(ILogger<TokenController> logger, ICacheService cacheService, IDatabaseService databaseService, IStorageService storageService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _databaseService = databaseService;
            _storageService = storageService;
        }

        [HttpGet("latestunclaimed")]
        public async Task<IActionResult> Get()
        {
            var dbEleme = await _cacheService.GetLatestCreatedTokens();

            return new OkObjectResult(dbEleme);
        }

        [HttpGet("details/{tokenAddress}")]
        public async Task<IActionResult> Get(string tokenAddress)
        {
            if (string.IsNullOrWhiteSpace(tokenAddress)) return BadRequest();

            var dbEleme = await _cacheService.GetTokenData(tokenAddress);

            return new OkObjectResult(dbEleme);
        }

        [HttpGet("socials/{tokenAddress}")]
        public async Task<IActionResult> GetSocials(string tokenAddress)
        {
            if (string.IsNullOrWhiteSpace(tokenAddress)) return BadRequest();

            var dbEleme = await _databaseService.GetSocialsByAddress(tokenAddress);

            return new OkObjectResult(dbEleme);
        }

        [HttpPost("submit-socials")]
        public async Task<ActionResult> SubmitSocials([FromForm] SubmitSocialsRequestModel model)
        {

            var bannerStoragePath = "";
            long size = 0;
            var fileName = "";

            if (model.Banner != null && model.Banner.Length > 0)
            {
                bool isImageFile = allowedImageFileExt.Contains(model.Banner.ContentType);
                if (!isImageFile)
                {
                    return BadRequest($"{model.Banner.ContentType} Not allowed");
                }

                size = model.Banner.Length;
                if (model.Banner.Length > 0)
                {
                    fileName = model.Banner.FileName.ToLowerInvariant();
                    await using var stream = model.Banner.OpenReadStream();
                    bannerStoragePath = await _storageService.UploadAsync(model.Contract, fileName, stream);
                }
            }

            _ = _databaseService.SaveSubmitedSocialsToken(new SubmitSocialsClaimModel
            {
                AssertOwned = true,
                Contract = model.Contract,
                Email = model.Email,
                Infringement = model.Infringement,
                Telegram = model.Telegram,
                Ticker = model.Ticker,
                TokenName = model.TokenName,
                Twitter = model.Twitter,
                Dexscreener = model.Dexscreener,
                Dextools = model.Dextools,
                Docs = model.Docs,
                Website = model.Website,
                BannerUrl = bannerStoragePath,
                TokenData = await _cacheService.GetTokenData(model.Contract),
            });

            _ = _databaseService.ApproveSubmitedSocialsToken(model.Contract);

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
