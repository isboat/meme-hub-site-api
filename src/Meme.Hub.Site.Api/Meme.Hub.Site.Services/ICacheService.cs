using Meme.Domain.Models.TokenModels;

namespace Meme.Hub.Site.Services
{
    public interface ICacheService
    {
        Task<List<TokenDataModel>> GetLatestCreatedTokens();

        Task<TokenDataModel> GetTokenData(string tokenAddress);

        Task CreateExpirationIndex();
    }
}