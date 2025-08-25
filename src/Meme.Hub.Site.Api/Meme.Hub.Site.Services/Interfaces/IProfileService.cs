using Amazon.Runtime.Internal;
using Meme.Hub.Site.Models.ProfileModels;
using MongoDB.Bson;

namespace Meme.Hub.Site.Services.Interfaces
{
    public interface IProfileService
    {
        Task<UserProfile?> GetProfile(string id);
        Task<List<UserProfile>?> GetKolsProfile();

        Task<bool> CreateProfile(UserProfile profile);

        Task AddFollower(string id, string followerId);

        Task RemoveFollower(string id, string followerId);
        Task UpdateProfile(string userId, UserProfile userProfile);

        Task EnableVerified(string userId);
        Task<List<UserProfile>> GetFollowers(string profileId);
        Task<List<UserProfile>> GetFollowing(string profileId);
    }

}
