
namespace Meme.Hub.Site.Models.MemeTokens
{
    public class TokenAddressDto
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public ChainDto Chain { get; set; }
        public string TokenAddress { get; set; }
        public string PairAddress { get; set; }
        public int TxnH24_Buy { get; set; }
        public int TxnH6_Buy { get; set; }
        public int TxnH1_Buy { get; set; }
        public int TxnH24_Sell { get; set; }
        public int TxnH6_Sell { get; set; }
        public int TxnH1_Sell { get; set; }
        public decimal VolumeH24 { get; set; }
        public decimal VolumeH6 { get; set; }
        public decimal VolumeH1 { get; set; }
        public DexDto Dex { get; set; }
        public object Launchpad { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
