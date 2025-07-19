using Meme.Hub.Site.Models.ProfileModels;
using Meme.Hub.Site.Services.Repository;
using MongoDB.Driver;

namespace Meme.Hub.Site.Services
{
    public interface IProfileService
    {
        Task<UserProfile> GetProfile(string id);
        Task<List<UserProfile>> GetKolsProfile();

        Task<bool> CreateProfile(UserProfile profile);
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
    }
}
