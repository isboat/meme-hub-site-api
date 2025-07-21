using AutoMapper;
using Meme.Domain.Models;
using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Models.ProfileModels;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Meme.Hub.Site.Services.Repository
{
    public interface ICosmosDBRepository
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);

        Task<IAsyncCursor<T>> GetData<T>(string collectionName, FilterDefinition<T> filterDefinition);

        Task Create<T>(T model, string collectionName);
    }

    public class CosmosDBRepository : ICosmosDBRepository
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public CosmosDBRepository(IOptions<MongoSettings> settings)
        {
            _client = new MongoClient(settings.Value.ConnectionString);
            _database = _client.GetDatabase(settings.Value.DatabaseName);
        }

        public Task Create<T>(T model, string collectionName)
        {
            _database.GetCollection<T>(collectionName).InsertOneAsync(model);
            return Task.CompletedTask;
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public Task<IAsyncCursor<T>> GetData<T>(string collectionName, FilterDefinition<T> filterDefinition)
        {
            return _database.GetCollection<T>(collectionName).FindAsync(filterDefinition);
        }
    }
}
