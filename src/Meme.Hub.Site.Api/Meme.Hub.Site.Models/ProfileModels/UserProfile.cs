namespace Meme.Hub.Site.Models.ProfileModels
{
    public class UserProfile : BaseProfile
    {
        public ProfileType? ProfileType { get; set; }

        public string? DiscountCode { get; set; }
        public string? Language { get; set; }
        public string? Location { get; set; }
    }
}
