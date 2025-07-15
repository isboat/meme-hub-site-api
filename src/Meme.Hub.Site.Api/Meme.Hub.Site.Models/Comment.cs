using Meme.Hub.Site.Models.Privy;
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
        public User User { get; set; } = new User();
        public bool IsNewUser { get; set; }
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
}
