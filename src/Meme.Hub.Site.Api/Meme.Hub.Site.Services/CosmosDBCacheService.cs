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

        public async Task<TokenDataModel> GetTokenData(string tokenAddress)
        {
            var items = await _database.GetCollection<TokenDataModel>(_collectionName).FindAsync(x => x.RawData != null && x.RawData.Mint == tokenAddress);

            return await items.FirstOrDefaultAsync();
        }

        public async Task<List<TokenDataModel>> GetLatestCreatedTokens()
        {
            var items = await _database.GetCollection<TokenDataModel>(_collectionName).FindAsync(x => x.CreatedDateTime > DateTime.Now.AddDays(-5));

            return items.ToList();
        }
    }
}
