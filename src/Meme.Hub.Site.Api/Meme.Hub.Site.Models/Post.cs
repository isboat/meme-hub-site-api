namespace Meme.Hub.Site.Models
{
    public class Post
    {
        public string _id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty; // ID of the user who posted
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public List<string> Likes { get; set; } = new List<string>(); // List of User _id's who liked
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
