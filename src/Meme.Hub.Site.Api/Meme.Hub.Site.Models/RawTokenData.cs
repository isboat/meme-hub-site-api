namespace Meme.Hub.Site.Models
{
    public class RawTokenData
    {
        public string Mint { get; set; } = string.Empty;
        public double InitialBuy { get; set; }
        public double MarketCapSol { get; set; }
        // Add other properties if they exist in your rawData
    }
}
