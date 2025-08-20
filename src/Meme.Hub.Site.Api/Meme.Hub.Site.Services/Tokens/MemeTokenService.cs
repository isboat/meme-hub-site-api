using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models.MemeTokens;
using Meme.Hub.Site.Services.Interfaces;
using Meme.Hub.Site.Services.Providers.Tokens;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Meme.Hub.Site.Services.Tokens
{
    public class MemeTokenService : IMemeTokenService
    {
        private readonly ITokenDataProvider _tokenDataProvider;

        public MemeTokenService(ITokenDataProvider tokenDataProvider)
        {
            _tokenDataProvider = tokenDataProvider;
        }

        public Task<JsonObject> GetCoinDataByIdAsync(string coinId)
        {
            return _tokenDataProvider.GetCoinDataByIdAsync(coinId);
        }

        public Task<IEnumerable<CoinGeckoTokenModel>> GetCoinsByNetwork(string network)
        {
            return _tokenDataProvider.GetCoinsByNetwork(network);
        }

        public Task<IEnumerable<TokenNetworkModel>> GetTokenNetworks()
        {
            return _tokenDataProvider.GetTokenNetworks();
        }

        public Task<IEnumerable<TokenDataModel>> GetTrendingTokens()
        {
            return _tokenDataProvider.GetTrendingTokens();
        }

        public Task<IEnumerable<CoinGeckoTokenModel>> SearchCoin(string search)
        {
            return _tokenDataProvider.SearchCoin(search);
        }
    }
}
