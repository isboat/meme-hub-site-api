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

        public TokenController(ILogger<TokenController> logger, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dbEleme = await _cacheService.GetItemsFromList();

            return new OkObjectResult(dbEleme);
        }
    }
}
