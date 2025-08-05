namespace Meme.Hub.Site.Models.ProfileModels
{
    public class BaseProfile
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? ProfileName { get; set; }

        public bool Verified { get; set; }

        public string? ProfileImage { get; set; }
        public string? Description { get; set; }
        public List<string> Followers { get; set; } = new List<string>(); // List of User _id's
        public List<string> Following { get; set; } = new List<string>(); // List of User _id's

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}
