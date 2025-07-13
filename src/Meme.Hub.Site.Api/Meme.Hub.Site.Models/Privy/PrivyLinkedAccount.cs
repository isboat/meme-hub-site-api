namespace Meme.Hub.Site.Models.Privy
{
    public class PrivyLinkedAccount
    {
        public string? Id { get; set; } // Optional, only appears for wallet
        public string? Address { get; set; }
        public string? Type { get; set; }
        public bool? Imported { get; set; }
        public bool? Delegated { get; set; }
        public DateTime VerifiedAt { get; set; }
        public DateTime FirstVerifiedAt { get; set; }
        public DateTime LatestVerifiedAt { get; set; }
        public string? ChainType { get; set; }
        public string? WalletClientType { get; set; }
        public string? ConnectorType { get; set; }
        public string? RecoveryMethod { get; set; }
        public int? WalletIndex { get; set; }
    }

}
