using Meme.Hub.Site.Models.ProfileModels;
using Meme.Hub.Site.Services.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Meme.Hub.Site.Services
{
    public interface IProfileService
    {
        Task<UserProfile> GetProfile(string id);
        Task<List<UserProfile>> GetKolsProfile();

        Task<bool> CreateProfile(UserProfile profile);

        Task AddFollower(string id, string followerId);

        Task RemoveFollower(string id, string followerId);
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
            return profile;
        }

        public async Task<List<UserProfile>> GetKolsProfile()
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.ProfileType, ProfileType.Kol);

            var profiles = (await _dbRepository.GetData(collectionName, filter));
            return profiles.ToList();
        }

        public Task<bool> CreateProfile(UserProfile profile)
        {
            profile.CreatedAt = DateTime.UtcNow;

            _dbRepository.GetCollection<UserProfile>(collectionName).InsertOneAsync(profile);
            return Task.FromResult(true);
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

        private async Task UpdateFollows(string id, UserProfile profile, IMongoCollection<UserProfile> collection)
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.Id, id);
            var update = Builders<UserProfile>.Update.Set(u => u.Followers, profile.Followers);

            var result = await collection.UpdateOneAsync(filter, update);
        }
    }
}
