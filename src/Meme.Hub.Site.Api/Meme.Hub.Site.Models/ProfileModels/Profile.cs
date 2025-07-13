namespace Meme.Hub.Site.Models.ProfileModels
{
    public class UserProfile
    {
        public string Id { get; set; }
        public ProfileType? Type { get; set; }
        public string UserId { get; set; }
        public string ProfileName { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public Dictionary<string,string> Metadata { get; set; }
    }

    public enum ProfileType
    {
        TokenLoverProfile,
        KolProfile,
        TokenDevProfile
    }
}
