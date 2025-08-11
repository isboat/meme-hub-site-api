using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Api.Models;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace Meme.Hub.Site.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        /// <summary>
        /// get all tokens
        /// </summary>
        /// <returns></returns>
        [HttpGet("createindex")]
        public async Task<IActionResult> GetIndexCreation()
        {
            await _cacheService.CreateExpirationIndex();

            return new OkResult();
        }

        /// <summary>
        /// get latest created tokens for a specific launch platform
        /// </summary>
        /// <param name="launchPlatform"></param>
        /// <returns></returns>
        [HttpGet("latestcreated/{launchPlatform}")]
        public async Task<IActionResult> GetLatestCreatedTokens(string launchPlatform)
        {
            switch (launchPlatform)
            {
                case "pumpfun":
                    var dbEleme = await _cacheService.GetLatestCreatedTokens();
                    return new OkObjectResult(dbEleme);
                default:
                    break;
            }

            return new OkObjectResult(new List<TokenDataModel>());

        }

        [HttpGet("latestunclaimed")]
        public async Task<IActionResult> Get(string platform)
        {
            var dbEleme = await _cacheService.GetLatestCreatedTokens();

            return new OkObjectResult(dbEleme);
        }

        [HttpGet("{tokenAddress}")]
        public async Task<IActionResult> GetTokenDetails(string tokenAddress)
        {
            if (string.IsNullOrWhiteSpace(tokenAddress)) return BadRequest();

            var dbEleme = await _cacheService.GetTokenData(tokenAddress);
            if (dbEleme == null)
            {
                return NotFound(new { message = "Token not found" });
            }

            return new OkObjectResult(dbEleme);
        }
    }
}
