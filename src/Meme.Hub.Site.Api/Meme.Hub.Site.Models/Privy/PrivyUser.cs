using System.ComponentModel.DataAnnotations;

namespace Meme.Hub.Site.Models.Privy
{
    public class PrivyUser
    {
        [Required]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<PrivyLinkedAccount> LinkedAccounts { get; set; }
        public PrivyEmailDetails Email { get; set; }
        public PrivyWalletDetails Wallet { get; set; }
        public List<object> DelegatedWallets { get; set; } // If needed, replace with concrete class
        public List<object> MfaMethods { get; set; }       // If needed, replace with concrete class
        public bool HasAcceptedTerms { get; set; }
        public bool IsGuest { get; set; }
    }
}
