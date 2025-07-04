namespace Meme.Hub.Site.Models
{
    public class Comment
    {
        public string _id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty; // ID of the user who commented
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
