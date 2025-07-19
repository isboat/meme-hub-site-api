using Meme.Hub.Site.Api.Models;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meme.Hub.Site.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemeTokenController : ControllerBase
    {
        private readonly IMemeTokenService memeTokenService;

        public MemeTokenController(IMemeTokenService memeTokenService)
        {
            this.memeTokenService = memeTokenService;
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
            },
            new UnclaimedToken
            {
                Name = "FomoBomb",
                Image = "https://via.placeholder.com/80/5733FF/FFFFFF?text=FB",
                RawData = new RawTokenData { Mint = "CxD1yE2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.08, MarketCapSol = 90000 }
            },
            new UnclaimedToken
            {
                Name = "YoloCoin",
                Image = "https://via.placeholder.com/80/FF33CC/FFFFFF?text=YC",
                RawData = new RawTokenData { Mint = "DyE1yF2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.2, MarketCapSol = 400000 }
            },
            new UnclaimedToken
            {
                Name = "SnipeX",
                Image = "https://via.placeholder.com/80/33CCFF/FFFFFF?text=SX",
                RawData = new RawTokenData { Mint = "EzF1yG2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.03, MarketCapSol = 75000 }
            },
            new UnclaimedToken
            {
                Name = "HypeDoge",
                Image = "https://via.placeholder.com/80/CCFF33/FFFFFF?text=HD",
                RawData = new RawTokenData { Mint = "FzG1yH2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.07, MarketCapSol = 180000 }
            },
            new UnclaimedToken
            {
                Name = "PepeEgg",
                Image = "https://via.placeholder.com/80/FF9933/FFFFFF?text=PE",
                RawData = new RawTokenData { Mint = "GzH1yI2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.06, MarketCapSol = 110000 }
            },
            new UnclaimedToken
            {
                Name = "ZoomRug",
                Image = "https://via.placeholder.com/80/9933FF/FFFFFF?text=ZR",
                RawData = new RawTokenData { Mint = "HzI1yJ2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.1, MarketCapSol = 60000 }
            },
            new UnclaimedToken
            {
                Name = "SolNeko",
                Image = "https://via.placeholder.com/80/33FF99/FFFFFF?text=SN",
                RawData = new RawTokenData { Mint = "IzJ1yK2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.04, MarketCapSol = 250000 }
            },
            new UnclaimedToken
            {
                Name = "MemeStorm",
                Image = "https://via.placeholder.com/80/FF3366/FFFFFF?text=MS",
                RawData = new RawTokenData { Mint = "JzK1yL2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.09, MarketCapSol = 130000 }
            },
            new UnclaimedToken
            {
                Name = "DogeFlip",
                Image = "https://via.placeholder.com/80/66FF33/FFFFFF?text=DF",
                RawData = new RawTokenData { Mint = "KzL1yM2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.11, MarketCapSol = 190000 }
            },
            new UnclaimedToken
            {
                Name = "ApeBlast",
                Image = "https://via.placeholder.com/80/3366FF/FFFFFF?text=AB",
                RawData = new RawTokenData { Mint = "LzM1yN2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.07, MarketCapSol = 100000 }
            },
            new UnclaimedToken
            {
                Name = "CatCoin",
                Image = "https://via.placeholder.com/80/CC33FF/FFFFFF?text=CC",
                RawData = new RawTokenData { Mint = "MzN1yO2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.05, MarketCapSol = 300000 }
            },
            new UnclaimedToken
            {
                Name = "Froggy",
                Image = "https://via.placeholder.com/80/33FFCC/FFFFFF?text=FR",
                RawData = new RawTokenData { Mint = "NzO1yP2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.15, MarketCapSol = 160000 }
            },
            new UnclaimedToken
            {
                Name = "TigerSwap",
                Image = "https://via.placeholder.com/80/FFCC33/FFFFFF?text=TS",
                RawData = new RawTokenData { Mint = "OzP1yQ2xN3b4C5d6E7f8G9h0I1j2K3l4M5n6O7p8Q9r0", InitialBuy = 0.08, MarketCapSol = 210000 }
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

        // You might have other controllers for Auth, Users, Posts etc.
    }
}
