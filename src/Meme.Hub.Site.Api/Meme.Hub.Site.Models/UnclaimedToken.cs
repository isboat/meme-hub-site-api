namespace Meme.Hub.Site.Models
{
    public class UnclaimedToken
    {
        public string _id { get; set; } = Guid.NewGuid().ToString(); // Unique ID for each token
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty; // URL to the image
        public RawTokenData RawData { get; set; } = new RawTokenData();
        // Add other properties if your backend sends them
    }
}
