using Amazon.Runtime.Internal;
using Meme.Hub.Site.Models.ProfileModels;
using Meme.Hub.Site.Services.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Meme.Hub.Site.Services.Interfaces
{
    public interface IProfileService
    {
        Task<UserProfile> GetProfile(string id);
        Task<List<UserProfile>> GetKolsProfile();

        Task<bool> CreateProfile(UserProfile profile);

        Task AddFollower(string id, string followerId);

        Task RemoveFollower(string id, string followerId);
        Task UpdateProfile(string userId, UserProfile userProfile);

        Task EnableVerified(string userId);
        Task<List<UserProfile>> GetFollowers(string profileId);
        Task<List<UserProfile>> GetFollowing(string profileId);
    }

    public class ProfileService : IProfileService
    {
        private readonly ICosmosDBRepository _dbRepository;
        private const string collectionName = $"{nameof(UserProfile)}s";

        public ProfileService(ICosmosDBRepository dbRepository)
        {
            _dbRepository = dbRepository;
        }

        public async Task<UserProfile> GetProfile(string id)
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.Id, id);

            var profile = (await _dbRepository.GetData(collectionName, filter)).FirstOrDefault();

            var followingFilter = Builders<UserProfile>.Filter.Where(u => u.Followers.Contains(profile.Id));
            var following = (await _dbRepository.GetData(collectionName, followingFilter)).ToList();
            profile.Following = following.Select(x => x.Id).ToList();

            return profile;
        }

        public async Task<List<UserProfile>> GetKolsProfile()
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.ProfileType, ProfileType.Kol);

            var profiles = await _dbRepository.GetData(collectionName, filter);
            return profiles.ToList();
        }

        public Task<bool> CreateProfile(UserProfile profile)
        {
            profile.CreatedAt = DateTime.UtcNow;
            GenerateDiscountCode(profile);

            _dbRepository.GetCollection<UserProfile>(collectionName).InsertOneAsync(profile);
            return Task.FromResult(true);
        }

        private void GenerateDiscountCode(UserProfile request)
        {
            if (request != null)
            {
                var guid = Guid.NewGuid().ToString("N").Replace("-", "");
                var code = string.IsNullOrEmpty(request?.ProfileName)
                    ? guid[..8]
                    : request.ProfileName.Replace(" ", "").Substring(0, 5) + guid.Substring(0, 3);

                request!.DiscountCode = code.ToUpperInvariant();
            }
        }

        public async Task AddFollower(string id, string followerId)
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.Id, id);

            var collection = _dbRepository.GetCollection<UserProfile>(collectionName);
            var profile = collection.Find(x => x.Id == id).FirstOrDefault();
            profile.Followers ??= [];

            if(!profile.Followers.Contains(followerId)) profile.Followers.Add(followerId);

            await UpdateFollows(id, profile, collection);
        }

        public async Task RemoveFollower(string id, string followerId)
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.Id, id);

            var collection = _dbRepository.GetCollection<UserProfile>(collectionName);
            var profile = collection.Find(x => x.Id == id).FirstOrDefault();
            profile.Followers ??= [];

            if(profile.Followers.Remove(followerId)) await UpdateFollows(id, profile, collection);
        }

        public async Task UpdateProfile(string userId, UserProfile userProfile)
        {
            // to be removed
            // GenerateDiscountCode(userProfile);
            var collection = _dbRepository.GetCollection<UserProfile>(collectionName);

            var filter = Builders<UserProfile>.Filter.Eq(u => u.Id, userId);
            var update = Builders<UserProfile>.Update
                .Set(u => u.LastUpdatedAt, DateTime.UtcNow);

            if(!string.IsNullOrEmpty(userProfile.Description))
            {
                update = update.Set(u => u.Description, userProfile.Description);
            }
            if(!string.IsNullOrEmpty(userProfile.ProfileName))
            {
                update = update.Set(u => u.ProfileName, userProfile.ProfileName);
            }
            if(!string.IsNullOrEmpty(userProfile.Location))
            {
                update = update.Set(u => u.Location, userProfile.Location);
            }
            if(!string.IsNullOrEmpty(userProfile.Language))
            {
                update = update.Set(u => u.Language, userProfile.Language);
            }
            if(!string.IsNullOrEmpty(userProfile.DiscountCode))
            {
                update = update.Set("DiscountCode", userProfile.DiscountCode);
            }
            if (!string.IsNullOrEmpty(userProfile.ProfileImage))
            {
                update = update.Set(u => u.ProfileImage, userProfile.ProfileImage);
            }
            if(userProfile.Metadata != null && userProfile.Metadata.Count > 0)
            {
                update = update.Set(u => u.Metadata, userProfile.Metadata);
            }

            _ = await collection.UpdateOneAsync(filter, update);
        }

        public async Task EnableVerified(string userId)
        {
            var collection = _dbRepository.GetCollection<UserProfile>(collectionName);

            var filter = Builders<UserProfile>.Filter.Eq(u => u.Id, userId);
            var update = Builders<UserProfile>.Update.Set("Verified", true);

            _ = await collection.UpdateOneAsync(filter, update);
        }

        private async Task UpdateFollows(string id, UserProfile profile, IMongoCollection<UserProfile> collection)
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.Id, id);
            var update = Builders<UserProfile>.Update.Set(u => u.Followers, profile.Followers);

            _ = await collection.UpdateOneAsync(filter, update);
        }

        public async Task<List<UserProfile>> GetFollowers(string profileId)
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.Id, profileId);
            var collection = _dbRepository.GetCollection<UserProfile>(collectionName);
            var profile = collection.Find(filter).FirstOrDefault();
            if (profile == null || profile.Followers == null || !profile.Followers.Any())
            {
                return await Task.FromResult(new List<UserProfile>());
            }
            var followersFilter = Builders<UserProfile>.Filter.In(u => u.Id, profile.Followers);
            return (await _dbRepository.GetData(collectionName, followersFilter)).ToList();
        }

        public async Task<List<UserProfile>> GetFollowing(string profileId)
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.Id, profileId);
            var collection = _dbRepository.GetCollection<UserProfile>(collectionName);
            var profile = collection.Find(filter).FirstOrDefault();
            if (profile == null)
            {
                return await Task.FromResult(new List<UserProfile>());
            }
            var followingFilter = Builders<UserProfile>.Filter.Where(u => u.Followers.Contains(profile.Id));
            return (await _dbRepository.GetData(collectionName, followingFilter)).ToList();
        }
    }
}
