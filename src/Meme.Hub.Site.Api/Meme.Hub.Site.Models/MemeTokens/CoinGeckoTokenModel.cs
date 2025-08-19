namespace Meme.Hub.Site.Models.MemeTokens
{
    public class CoinGeckoTokenModel
    {
        public string ChainId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Marketcap { get; set; }
        public string Price { get; set; }
        public string LogoURI { get; set; }

        public TokenAddressDto AddressDto { get; set; }
    }
}