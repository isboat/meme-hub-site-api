
namespace Meme.Hub.Site.Models.MemeTokens
{
    public class ChainDto
    {
        public string Id { get; set; }
        public string ChainId { get; set; }
        public string Name { get; set; }
        public string Abbr { get; set; }
        public string Slug { get; set; }
        public string CurrencySymbol { get; set; }
        public string ExplorerUrl { get; set; }
        public string DextoolsIdentifier { get; set; }
        public string GeckoterminalIdentifier { get; set; }
        public string PayCurrency { get; set; }
        public string Emoji { get; set; }
        public string LogoUrl { get; set; }
        public decimal PriceUsd { get; set; }
        public DateTime PriceUpdatedAt { get; set; }
        public int Ranking { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
