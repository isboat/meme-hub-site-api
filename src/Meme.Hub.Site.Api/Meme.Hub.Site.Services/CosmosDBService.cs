using Amazon;
using AutoMapper;
using Meme.Domain.Models;
using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Models.ProfileModels;
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
        private readonly string _submitSocialsColName = $"{nameof(SocialsClaimModel)}s";
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

        public async Task<string> SaveSubmitedSocialsToken(SocialsClaimModel submitTokenClaim)
        {
            // MTH12345
            var id =  $"MTH{GenerateId()}";
            submitTokenClaim.Id = id;
            await _database.GetCollection<SocialsClaimModel>(_submitSocialsColName).InsertOneAsync(submitTokenClaim);
            return id;
        }

        private static string GenerateId()
        {
            var currenttime = DateTime.UtcNow;
            return $"{currenttime.Millisecond}{currenttime.Microsecond}";
        }

        public async Task<IEnumerable<SocialsClaimModel>?> GetUserPendingSocialsClaims(string userId)
        {
            var collection = _database.GetCollection<SocialsClaimModel>(_submitSocialsColName);
            var submittedClaims = await (await collection.FindAsync(x => x.UserId == userId && x.Status == SocialsClaimStatus.Pending)).ToListAsync();
            return submittedClaims;
        }

        public async Task<IEnumerable<SocialsClaimModel>?> GetSocialClaims()
        {
            var collection = _database.GetCollection<SocialsClaimModel>(_submitSocialsColName);
            var claims = await(await collection.FindAsync(x => x.Status == SocialsClaimStatus.Pending || x.Status == SocialsClaimStatus.Approved)).ToListAsync();
            return claims;
        }

        public async Task<bool> ApproveSubmitedSocialsToken(string id, string approverUserId)
        {
            var collection = _database.GetCollection<SocialsClaimModel>(_submitSocialsColName);
            
            var submittedClaim = collection.Find(x => x.Id == id).FirstOrDefault();
            if (submittedClaim == null) return false;

            submittedClaim.Approvers ??= [];
            if(!submittedClaim.Approvers.Any(x => x.UserId == approverUserId))
            {
                submittedClaim.Approvers.Add(new Approval
                {
                    UserId = approverUserId,
                    ApprovedAt = DateTime.UtcNow,
                });
            }

            var filter = Builders<SocialsClaimModel>.Filter.Eq(u => u.TokenAddress, id);
            var update = Builders<SocialsClaimModel>.Update.Set(u => u.Approvers, submittedClaim.Approvers);

            _ = await collection.UpdateOneAsync(filter, update);

            return true;
        }

        public async Task<SocialsClaimModel> GetTokenSocialsClaimById(string claimId)
        {
            return (await _database.GetCollection<SocialsClaimModel>(_submitSocialsColName).FindAsync(x => x.Id == claimId)).FirstOrDefault();
        }

        public async Task<SocialsClaimModel> GetTokenSocialsClaimByTokenAddress(string tokenAddress)
        {
            return (await _database.GetCollection<SocialsClaimModel>(_submitSocialsColName).FindAsync(x => x.TokenAddress == tokenAddress)).FirstOrDefault();
        }
    }
}
