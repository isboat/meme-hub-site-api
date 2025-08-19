using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models.MemeTokens;
using System.Text.Json.Nodes;

namespace Meme.Hub.Site.Services.Interfaces
{
    public interface IMemeTokenService
    {
        Task<IEnumerable<TokenNetworkModel>> GetTokenNetworks();

        Task<JsonObject> GetCoinDataByIdAsync(string coinId);

        Task<IEnumerable<TokenDataModel>> GetTrendingTokens();

        Task<IEnumerable<CoinGeckoTokenModel>> GetCoinsByNetwork(string network);
    }
}
