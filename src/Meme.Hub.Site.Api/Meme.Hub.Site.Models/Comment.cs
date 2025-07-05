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

    public class GetOrRegisterUserRequestDto
    {
        [Required]
        public string PrivyId { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PrivyAccessToken { get; set; } = string.Empty; // Privy's JWT

        public string? WalletAddress { get; set; } // Optional: if you're sending wallet address
    }

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public User User { get; set; } = new User(); // The full user object from your backend
    }
}
