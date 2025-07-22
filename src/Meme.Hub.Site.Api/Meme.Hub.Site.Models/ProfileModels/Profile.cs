namespace Meme.Hub.Site.Models.ProfileModels
{
    public class UserProfile
    {
        public string Id { get; set; }
        public ProfileType? ProfileType { get; set; }
        public string UserId { get; set; }
        public string? ProfileName { get; set; }
        public string? ProfileImage { get; set; }
        public string? Description { get; set; }
        public List<string> Followers { get; set; } = new List<string>(); // List of User _id's
        public List<string> Following { get; set; } = new List<string>(); // List of User _id's

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public Dictionary<string,string> Metadata { get; set; }
        public string Language { get; set; }
        public string Location { get; set; }
    }

    public enum ProfileType
    {
        Lover,
        Kol,
        Dev
    }
}
