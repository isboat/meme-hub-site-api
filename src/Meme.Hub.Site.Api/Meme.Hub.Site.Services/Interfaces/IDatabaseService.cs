using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models;

namespace Meme.Hub.Site.Services.Interfaces
{
    public interface IDatabaseService
    {
        Task<TokenDataModel> GetTokenData(string tokenAddress);

        Task<bool> SaveSubmitedSocialsToken(SocialsClaimModel submitTokenClaim);

        Task<IEnumerable<SocialsClaimModel>?> GetUserPendingSocialsClaims(string userId);

        Task<bool> ApproveSubmitedSocialsToken(string tokenAddress, string approverUserId);
        Task<SocialsClaimModel> GetTokenSocialsClaimById(string claimId);
    }
}