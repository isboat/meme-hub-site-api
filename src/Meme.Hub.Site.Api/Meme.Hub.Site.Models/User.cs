namespace Meme.Hub.Site.Models
{
    public class User
    {
        public string _id { get; set; } = Guid.NewGuid().ToString(); // Internal database ID
        public string PrivyId { get; set; } = string.Empty; // ID from Privy.io
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }
        public SocialLinks SocialLinks { get; set; } = new SocialLinks();
        public List<string> Followers { get; set; } = new List<string>(); // List of User _id's
        public List<string> Following { get; set; } = new List<string>(); // List of User _id's
        public UserSettings Settings { get; set; } = new UserSettings();

        public string? RefreshToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public PrivyUser PrivyUserDetails { get; set; }
    }
}
