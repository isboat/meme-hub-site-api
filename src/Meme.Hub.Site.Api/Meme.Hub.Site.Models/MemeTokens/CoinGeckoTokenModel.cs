namespace Meme.Hub.Site.Models.MemeTokens
{
    public class CoinGeckoTokenModel
    {
        public string ChainId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public string LogoURI { get; set; }
    }
}