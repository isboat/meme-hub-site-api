namespace Meme.Hub.Site.Models.Privy
{
    public class PrivyWalletDetails
    {
        public string? Id { get; set; }
        public string? Address { get; set; }
        public string? ChainType { get; set; }
        public string? WalletClientType { get; set; }
        public string? ConnectorType { get; set; }
        public string? RecoveryMethod { get; set; }
        public bool Imported { get; set; }
        public bool Delegated { get; set; }
        public int WalletIndex { get; set; }
    }

}
