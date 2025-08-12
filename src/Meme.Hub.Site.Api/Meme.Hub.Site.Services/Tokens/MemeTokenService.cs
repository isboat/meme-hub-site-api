using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models.MemeTokens;
using Meme.Hub.Site.Services.Interfaces;
using Meme.Hub.Site.Services.Providers.Tokens;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Meme.Hub.Site.Services.Tokens
{
    public class CoinGeckoMemeTokenService : ICoinGeckoService
    {
        private readonly ICoinGeckoProvider _coinGeckoProvider;

        public CoinGeckoMemeTokenService(ICoinGeckoProvider coinGeckoProvider)
        {
            _coinGeckoProvider = coinGeckoProvider;
        }

        public Task<string> GetCoinDataByIdAsync(string coinId)
        {
            return _coinGeckoProvider.GetCoinDataByIdAsync(coinId);
        }

        public Task<List<TokenNetworkModel>> GetTokenNetworks()
        {
            return _coinGeckoProvider.GetTokenNetworks();
        }

        public Task<List<CoinGeckoTokenModel>> GetTokensByNetworkId(string networkId)
        {
            return _coinGeckoProvider.GetTokensByNetworkId(networkId);
        }
    }

    public class MemeTokenService : IMemeTokenService
    {
        private readonly ITokenDataProvider _tokenDataProvider;
        private readonly ICoinGeckoService _coinGeckoService;

        public MemeTokenService(ITokenDataProvider tokenDataProvider, ICoinGeckoService coinGeckoService)
        {
            _tokenDataProvider = tokenDataProvider;
            _coinGeckoService = coinGeckoService;
        }

        public Task<JsonObject> GetCoinDataByIdAsync(string coinId)
        {
            return _tokenDataProvider.GetCoinDataByIdAsync(coinId);
        }

        public Task<IEnumerable<TokenDetailsDto>> GetCoinsByNetwork(string network)
        {
            return _tokenDataProvider.GetCoinsByNetwork(network);
        }

        public Task<IEnumerable<TokenChains>> GetTokenNetworks()
        {
            return _tokenDataProvider.GetTokenNetworks();
        }

        public Task<IEnumerable<TokenDataModel>> GetTrendingTokens()
        {
            return _tokenDataProvider.GetTrendingTokens();
        }
    }
}
