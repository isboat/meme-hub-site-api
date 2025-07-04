namespace Meme.Hub.Site.Models
{
    public class TickerItem
    {
        public string _id { get; set; } = Guid.NewGuid().ToString(); // Matches TypeScript _id
        public string Text { get; set; } = string.Empty;
        public string? Link { get; set; } // Nullable string for optional link
    }
}
