namespace Meme.Hub.Site.Models
{
    // DTO for creating a new post
    public class CreatePostDto
    {
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
