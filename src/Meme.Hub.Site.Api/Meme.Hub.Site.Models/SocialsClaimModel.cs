using Meme.Domain.Models.TokenModels;

namespace Meme.Hub.Site.Models
{
    public class SocialsClaimModel
    {
        public string Id { get; set; }
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
        public string Others { get; set; }
        public string DiscordUsername { get; set; }
        public string TelegramUsername { get; set; }
        public string BannerUrl { get; set; }
        public string LogoUrl { get; set; }
        public TokenDataModel TokenData { get; set; }
        public List<Approval> Approvers { get; set; }
        public DateTime SubmitedAt { get; set; }
        public SocialsClaimStatus Status { get; set; }
    }

    public class Approval
    {
        public string UserId { get; set; }
        public DateTime? ApprovedAt { get; set; }

    }

    public enum SocialsClaimStatus
    {
        Pending,
        Approved,
        Cancelled
    }
}
