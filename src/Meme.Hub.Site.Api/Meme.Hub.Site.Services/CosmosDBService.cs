using Meme.Domain.Models;
using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meme.Hub.Site.Services
{
    public class CosmosDBService : IDatabaseService
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly string _collectionName;
        private readonly string _submitSocialsColName = $"{nameof(SubmitSocialsClaimModel)}s";

        public CosmosDBService(IOptions<MongoSettings> settings)
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

        public async Task<bool> SaveSubmitedSocialsToken(SubmitSocialsClaimModel submitTokenClaim)
        {
            submitTokenClaim.Id = Guid.NewGuid().ToString("N");
            await _database.GetCollection<SubmitSocialsClaimModel>(_submitSocialsColName).InsertOneAsync(submitTokenClaim);
            return true;
        }
    }
}
