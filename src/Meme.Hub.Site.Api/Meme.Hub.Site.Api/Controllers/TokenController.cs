using Meme.Hub.Site.Api.Models;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services;
using Microsoft.AspNetCore.Mvc;

namespace Meme.Hub.Site.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {

        private readonly ILogger<TokenController> _logger;

        private readonly ICacheService _cacheService;
        private readonly IDatabaseService _databaseService;

        public TokenController(ILogger<TokenController> logger, ICacheService cacheService, IDatabaseService databaseService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _databaseService = databaseService;
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

        [HttpPost("submit-socials")]
        public async Task<ActionResult> SubmitSocials([FromForm] SubmitSocialsRequestModel model)
        {
            if (model.Banner != null && model.Banner.Length > 0)
            {
                var filePath = Path.Combine("uploads", model.Banner.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Banner.CopyToAsync(stream);
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
                BannerUrl = "testurl.com",
                TokenData = await _cacheService.GetTokenData(model.Contract),
            });

            return Ok("Form submitted successfully!");
        }

    }
}
