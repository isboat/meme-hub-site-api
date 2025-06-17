using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models;

namespace Meme.Hub.Site.Services
{
    public interface IDatabaseService
    {
        Task<TokenDataModel> GetTokenData(string tokenAddress);

        Task<bool> SaveSubmitedSocialsToken(SubmitSocialsClaimModel submitTokenClaim);
    }
}