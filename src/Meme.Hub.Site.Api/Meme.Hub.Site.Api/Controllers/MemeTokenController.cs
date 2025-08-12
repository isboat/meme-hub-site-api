using Meme.Hub.Site.Api.Models;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meme.Hub.Site.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemeTokenController : ControllerBase
    {
        private readonly IMemeTokenService memeTokenService;
        private readonly ICoinGeckoService _coinGeckoService;

        public MemeTokenController(IMemeTokenService memeTokenService, ICoinGeckoService coinGeckoService)
        {
            this.memeTokenService = memeTokenService;
            _coinGeckoService = coinGeckoService;
        }

        // --- Dummy Data (replace with database calls in a real application) ---
        private static readonly List<TickerItem> _tickerData = new List<TickerItem>
        {
            new TickerItem { Text = "Breaking News: SocialSphere hits 1M users!", Link = "/news/1" },
            new TickerItem { Text = "New Feature: Anonymous Browse is now live!", Link = "/features/anon" },
            new TickerItem { Text = "Community Event: Virtual Meetup on July 20th!", Link = "/events/meetup" },
            new TickerItem { Text = "Tip: Update your profile links in settings!", Link = "/settings" },
        };

        private static readonly List<UnclaimedToken> _unclaimedTokensData = new List<UnclaimedToken>
        {
            new UnclaimedToken
            {
                Name = "SolPepe",
                Image = "https://via.placeholder.com/80/FF5733/FFFFFF?text=SP",
                RawData = new RawTokenData { Mint = "AyZ1yV2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.05, MarketCapSol = 150000 }
            },
            new UnclaimedToken
            {
                Name = "MoonDuck",
                Image = "https://via.placeholder.com/80/33FF57/FFFFFF?text=MD",
                RawData = new RawTokenData { Mint = "BzC1yD2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.12, MarketCapSol = 230000 }
            }
        };

        // GET: /api/ticker
        [HttpGet("ticker")]
        public ActionResult<IEnumerable<TickerItem>> GetTickerItems()
        {
            // In a real app, you'd fetch this from a service or database
            return Ok(_tickerData);
        }

        // GET: /api/token/latestunclaimed
        [HttpGet("latestunclaimed")]
        public ActionResult<IEnumerable<UnclaimedToken>> GetLatestUnclaimedTokens()
        {
            // In a real app, you'd fetch this from a service or database
            return Ok(_unclaimedTokensData);
        }



        // GET: /api/token/latestunclaimed
        [HttpGet("trending")]
        public async Task<ActionResult> GetLatestTrendingTokens()
        {
            // In a real app, you'd fetch this from a service or database
            var data = await memeTokenService.GetTrendingTokens();
            return Ok(data);
        }

        // GET: /api/token/latestunclaimed
        [HttpGet("networks")]
        public async Task<ActionResult> GetTokenNetworks()
        {
            // In a real app, you'd fetch this from a service or database
            var data = await _coinGeckoService.GetTokenNetworks();
            return Ok(data);
        }

        // GET: /api/token/latestunclaimed
        [HttpGet("{network}/tokens")]
        public async Task<ActionResult> GetTokenNetworks(string network)
        {
            // In a real app, you'd fetch this from a service or database
            var data = await _coinGeckoService.GetTokensByNetworkId(network);
            return Ok(data);
        }
    }
}
