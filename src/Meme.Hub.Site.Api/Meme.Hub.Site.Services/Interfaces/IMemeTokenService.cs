using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models.MemeTokens;
using System.Text.Json.Nodes;

namespace Meme.Hub.Site.Services.Interfaces
{
    public interface IMemeTokenService
    {
        Task<IEnumerable<TokenChains>> GetTokenNetworks();

        Task<JsonObject> GetCoinDataByIdAsync(string coinId);

        Task<IEnumerable<TokenDataModel>> GetTrendingTokens();

        Task<IEnumerable<TokenDetailsDto>> GetCoinsByNetwork(string network);
    }
    public interface ICoinGeckoService
    {

        Task<List<TokenNetworkModel>> GetTokenNetworks();

        Task<List<CoinGeckoTokenModel>> GetTokensByNetworkId(string networkId);

        Task<string> GetCoinDataByIdAsync(string coinId);
    }
}
