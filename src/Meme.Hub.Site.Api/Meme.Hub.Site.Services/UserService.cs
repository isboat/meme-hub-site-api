using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services.Interfaces;
using Meme.Hub.Site.Services.Repository;
using MongoDB.Driver;

namespace Meme.Hub.Site.Services
{
    public class UserService(ICosmosDBRepository dbRepository) : IUserService
    {
        private readonly ICosmosDBRepository _dbRepository = dbRepository;
        private const string collectionName = $"{nameof(User)}s";

        public async Task<bool> CreateUserAsync(User user)
        {
            var collection = _dbRepository.GetCollection<User>(collectionName);
            await collection.InsertOneAsync(user);
            return true;
        }

        public async Task<User> GetUserByPrivyId(string privyId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.PrivyId, privyId);

            var user = (await _dbRepository.GetData(collectionName, filter)).FirstOrDefault();
            return user;
        }
    }
}
