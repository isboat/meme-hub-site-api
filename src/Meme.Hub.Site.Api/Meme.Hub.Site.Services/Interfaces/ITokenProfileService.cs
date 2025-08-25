using Meme.Hub.Site.Models.ProfileModels;

namespace Meme.Hub.Site.Services.Interfaces
{
    public interface ITokenProfileService
    {
        Task<TokenProfile?> GetProfile(string id);

        Task<bool> CreateProfile(TokenProfile profile);

        Task AddFollower(string id, string followerId);

        Task RemoveFollower(string id, string followerId);
        Task UpdateProfile(string userId, TokenProfile userProfile);

        Task EnableVerified(string userId);
        Task<List<TokenProfile>> GetFollowers(string profileId);
        Task<List<TokenProfile>> GetFollowing(string profileId);
    }

}
