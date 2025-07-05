using System.ComponentModel.DataAnnotations;

namespace Meme.Hub.Site.Api.Models
{
    // DTO for refresh token request
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
