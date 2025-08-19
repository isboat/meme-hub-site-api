
namespace Meme.Hub.Site.Models.MemeTokens
{
    public class MemeokenData
    {

    }

    public class TokenChains
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

    public class TokenDetailsDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
            public string Symbol { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
            public string LogoUrl { get; set; }
            public decimal Price { get; set; }
            public decimal Marketcap { get; set; }
            public decimal Liquidity { get; set; }
            public decimal PriceChangeH1 { get; set; }
            public decimal PriceChangeH6 { get; set; }
            public decimal PriceChangeH24 { get; set; }
            public DateTime ListedAt { get; set; }
            public bool IsExpressListing { get; set; }
            public int Status { get; set; }
            public DateTime LaunchedAt { get; set; }
            public string CreatedBy { get; set; }
            public List<TokenAddressDto> Addresses { get; set; }
            public List<TokenLinkDto> Links { get; set; }
    }

    public class TokenLinkDto
    {
        public string Id { get; set; }
        public TokenLinkType Type { get; set; }
        public string Url { get; set; }
    }

    public class TokenLinkType
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
    }
}
