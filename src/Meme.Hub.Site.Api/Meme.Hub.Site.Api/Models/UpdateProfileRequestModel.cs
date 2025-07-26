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

    public class UpdateSocialsRequestModel
    {
        public string X { get; set; }
        public string Telegram { get; set; }
        public string Youtube { get; set; }
        public string Instagram { get; set; }
    }
}
