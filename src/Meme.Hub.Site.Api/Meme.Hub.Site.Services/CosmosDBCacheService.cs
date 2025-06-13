using Meme.Domain.Models;
using Meme.Domain.Models.TokenModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Meme.Hub.Site.Services
{
    public class CosmosDBCacheService: ICacheService
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly string _collectionName;

        public CosmosDBCacheService(IOptions<MongoSettings> settings)
        {
            _client = new MongoClient(settings.Value.ConnectionString);
            _database = _client.GetDatabase(settings.Value.DatabaseName);
            _collectionName = settings.Value.CollectionName;
        }

        public async Task<List<TokenDataModel>> GetItemsFromList()
        {
            var items = await _database.GetCollection<MongoTokenEntity>(_collectionName).FindAsync(x => x.CreatedDateTime > DateTime.Now.AddDays(-1));

            var tokenDataModels = (await items.ToListAsync()).Select(x => JsonConvert.DeserializeObject<TokenDataModel>(x.StrValue)).ToList();

            return tokenDataModels;
        }

        public async Task RemoveExpiredItemsAsync()
        {
        }

        public T? GetData<T>(string key)
        {
            var item = _database.GetCollection<MongoTokenEntity>(_collectionName).Find(x => x.Id == key).FirstOrDefault();

            if (item != null)
            {
                var value = item.StrValue;
                return JsonConvert.DeserializeObject<T>(value!);
            }
            return default;
        }

        public bool RemoveData(string key)
        {
            var result = _database.GetCollection<MongoTokenEntity>(_collectionName).DeleteOne(x => x.Id == key);
            return result.IsAcknowledged;
        }
    }
}
