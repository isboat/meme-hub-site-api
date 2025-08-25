using Meme.Hub.Site.Models.ProfileModels;
using Meme.Hub.Site.Services.Interfaces;
using Meme.Hub.Site.Services.Repository;
using MongoDB.Driver;

namespace Meme.Hub.Site.Services
{
    public class TokenProfileService : ITokenProfileService
    {
        private readonly ICosmosDBRepository _dbRepository;
        private const string collectionName = $"{nameof(TokenProfile)}s";

        public TokenProfileService(ICosmosDBRepository dbRepository)
        {
            _dbRepository = dbRepository;
        }

        public async Task<TokenProfile?> GetProfile(string id)
        {
            var filter = Builders<TokenProfile>.Filter.Eq(u => u.Id, id);

            var profile = (await _dbRepository.GetData(collectionName, filter)).FirstOrDefault();
            if (profile == null) return null;

            var followingFilter = Builders<TokenProfile>.Filter.Where(u => u.Followers.Contains(profile.Id!));
            var following = (await _dbRepository.GetData(collectionName, followingFilter))?.ToList();
            if (following != null && following.Count != 0)
            {
                profile.Following = [.. following.Select(x => x.Id)];
            }
            return profile;
        }

        public Task<bool> CreateProfile(TokenProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);
            if (string.IsNullOrEmpty(profile.Id)) throw new ArgumentException("Profile ID cannot be null or empty.", nameof(profile.Id));

            profile.CreatedAt = DateTime.UtcNow;

            _dbRepository.GetCollection<TokenProfile>(collectionName).InsertOneAsync(profile);
            return Task.FromResult(true);
        }

        public async Task AddFollower(string id, string followerId)
        {
            var filter = Builders<TokenProfile>.Filter.Eq(u => u.Id, id);

            var collection = _dbRepository.GetCollection<TokenProfile>(collectionName);
            var profile = collection.Find(x => x.Id == id).FirstOrDefault();
            profile.Followers ??= [];

            if(!profile.Followers.Contains(followerId)) profile.Followers.Add(followerId);

            await UpdateFollows(id, profile, collection);
        }

        public async Task RemoveFollower(string id, string followerId)
        {
            var filter = Builders<TokenProfile>.Filter.Eq(u => u.Id, id);

            var collection = _dbRepository.GetCollection<TokenProfile>(collectionName);
            var profile = collection.Find(x => x.Id == id).FirstOrDefault();
            profile.Followers ??= [];

            if(profile.Followers.Remove(followerId)) await UpdateFollows(id, profile, collection);
        }

        public async Task UpdateProfile(string userId, TokenProfile profile)
        {
            // to be removed
            // GenerateDiscountCode(userProfile);
            var collection = _dbRepository.GetCollection<TokenProfile>(collectionName);

            var filter = Builders<TokenProfile>.Filter.Eq(u => u.Id, userId);
            var update = Builders<TokenProfile>.Update
                .Set(u => u.LastUpdatedAt, DateTime.UtcNow);

            if(!string.IsNullOrEmpty(profile.Description))
            {
                update = update.Set(u => u.Description, profile.Description);
            }
            if(!string.IsNullOrEmpty(profile.ProfileName))
            {
                update = update.Set(u => u.ProfileName, profile.ProfileName);
            }
            if (!string.IsNullOrEmpty(profile.ProfileImage))
            {
                update = update.Set(u => u.ProfileImage, profile.ProfileImage);
            }
            if(profile.Metadata != null && profile.Metadata.Count > 0)
            {
                update = update.Set(u => u.Metadata, profile.Metadata);
            }

            _ = await collection.UpdateOneAsync(filter, update);
        }

        public async Task EnableVerified(string userId)
        {
            var collection = _dbRepository.GetCollection<TokenProfile>(collectionName);

            var filter = Builders<TokenProfile>.Filter.Eq(u => u.Id, userId);
            var update = Builders<TokenProfile>.Update.Set("Verified", true);

            _ = await collection.UpdateOneAsync(filter, update);
        }

        private async Task UpdateFollows(string id, TokenProfile profile, IMongoCollection<TokenProfile> collection)
        {
            var filter = Builders<TokenProfile>.Filter.Eq(u => u.Id, id);
            var update = Builders<TokenProfile>.Update.Set(u => u.Followers, profile.Followers);

            _ = await collection.UpdateOneAsync(filter, update);
        }

        public async Task<List<TokenProfile>> GetFollowers(string profileId)
        {
            var filter = Builders<TokenProfile>.Filter.Eq(u => u.Id, profileId);
            var collection = _dbRepository.GetCollection<TokenProfile>(collectionName);
            var profile = collection.Find(filter).FirstOrDefault();
            if (profile == null || profile.Followers == null || !profile.Followers.Any())
            {
                return await Task.FromResult(new List<TokenProfile>());
            }
            var followersFilter = Builders<TokenProfile>.Filter.In(u => u.Id, profile.Followers);
            return (await _dbRepository.GetData(collectionName, followersFilter)).ToList();
        }

        public async Task<List<TokenProfile>> GetFollowing(string profileId)
        {
            var filter = Builders<TokenProfile>.Filter.Eq(u => u.Id, profileId);
            var collection = _dbRepository.GetCollection<TokenProfile>(collectionName);
            var profile = collection.Find(filter).FirstOrDefault();
            if (profile == null)
            {
                return await Task.FromResult(new List<TokenProfile>());
            }
            var followingFilter = Builders<TokenProfile>.Filter.Where(u => u.Followers.Contains(profile.Id));
            return (await _dbRepository.GetData(collectionName, followingFilter)).ToList();
        }
    }
}
