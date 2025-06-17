namespace Meme.Hub.Site.Api.Models
{
    public class SubmitSocialsRequestModel
    {
        public string TokenName { get; set; }
        public string Ticker { get; set; }
        public string Contract { get; set; }
        public string Twitter { get; set; }
        public string Telegram { get; set; }
        public string Email { get; set; }
        public IFormFile Banner { get; set; } // File upload
        public bool Infringement { get; set; }
        public bool AssertOwned { get; set; }
    }
}
