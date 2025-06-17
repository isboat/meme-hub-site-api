using Meme.Domain.Models.TokenModels;

namespace Meme.Hub.Site.Models
{
    public class SubmitSocialsClaimModel
    {
        public string Id { get; set; }
        public string TokenName { get; set; }
        public string Ticker { get; set; }
        public string Contract { get; set; }
        public string Twitter { get; set; }
        public string Telegram { get; set; }
        public string Email { get; set; }
        public string BannerUrl { get; set; }
        public bool Infringement { get; set; }
        public bool AssertOwned { get; set; }
        public TokenDataModel TokenData { get; set; }
    }
}
