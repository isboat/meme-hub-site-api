using AutoMapper;
using Meme.Domain.Models;
using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services.Interfaces;
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
        private readonly string _approvedSocialsColName = $"{nameof(ApprovedSocialsModel)}s";
        private readonly IMapper _mapper;

        public CosmosDBService(IOptions<MongoSettings> settings, IMapper mapper)
        {
            _client = new MongoClient(settings.Value.ConnectionString);
            _database = _client.GetDatabase(settings.Value.DatabaseName);
            _collectionName = settings.Value.CollectionName;
            _mapper = mapper;
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

        public async Task<bool> ApproveSubmitedSocialsToken(string tokenAddress)
        {
            var submittedSocials = await ( await _database.GetCollection<SubmitSocialsClaimModel>(_submitSocialsColName).FindAsync(x => x.Contract == tokenAddress)).FirstOrDefaultAsync();

            if(submittedSocials == null) return false;

            var approved = _mapper.Map<ApprovedSocialsModel>(submittedSocials);
            if (approved == null) return false;

            await _database.GetCollection<ApprovedSocialsModel>(_approvedSocialsColName).InsertOneAsync(approved);

            return true;
        }

        public async Task<ApprovedSocialsModel> GetSocialsByAddress(string tokenAddress)
        {
            return (await _database.GetCollection<ApprovedSocialsModel>(_approvedSocialsColName).FindAsync(x => x.Contract == tokenAddress)).FirstOrDefault();
        }
    }
}
