namespace Meme.Hub.Site.Api.Models
{
    public class UpdateProfileRequestModel
    {
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string Language { get; set; }
        public IFormFile? ProfileImageFile { get; set; } // File upload
    }
}
