using System.ComponentModel.DataAnnotations;

namespace Meme.Hub.Site.Models
{
    public class Comment
    {
        public string _id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty; // ID of the user who commented
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public User User { get; set; } = new User(); // The full user object from your backend
    }

    ///

    public class GetTokenRequestDto
    {
        [Required]
        public string PrivyId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PrivyAccessToken { get; set; }

        public string? WalletAddress { get; set; }

        [Required]
        public PrivyUser PrivyUser { get; set; }
    }

    public class PrivyUser
    {
        [Required]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<LinkedAccount> LinkedAccounts { get; set; }
        public EmailDetails Email { get; set; }
        public WalletDetails Wallet { get; set; }
        public List<object> DelegatedWallets { get; set; } // If needed, replace with concrete class
        public List<object> MfaMethods { get; set; }       // If needed, replace with concrete class
        public bool HasAcceptedTerms { get; set; }
        public bool IsGuest { get; set; }
    }

    public class LinkedAccount
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

    public class EmailDetails
    {
        public string? Address { get; set; }
    }

    public class WalletDetails
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
