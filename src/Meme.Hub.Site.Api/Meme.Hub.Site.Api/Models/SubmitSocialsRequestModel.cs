namespace Meme.Hub.Site.Api.Models
{
    public class SubmitSocialsRequestModel
    {
        public string TokenName { get; set; }
        public string TokenAddress { get; set; }
        public string Chain { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string Twitter { get; set; }
        public string Telegram { get; set; }
        public string Discord { get; set; }
        public string Website { get; set; }
        public string Reddit { get; set; }
        public string Other { get; set; }
        public string DiscordUsername { get; set; }
        public string TelegramUsername { get; set; }
        public IFormFile ProfileImageFile { get; set; } // File upload
        public IFormFile ProfileLogoImageFile { get; set; } // File upload
    }
}
