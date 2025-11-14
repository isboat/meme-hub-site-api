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
        private readonly IDatabaseService _databaseService;

        public MemeTokenService(ITokenDataProvider tokenDataProvider, IDatabaseService databaseService)
        {
            _tokenDataProvider = tokenDataProvider;
            _databaseService = databaseService;
        }

        public Task<JsonObject> GetCoinDataByIdAsync(string coinId)
        {
            return _tokenDataProvider.GetCoinDataByIdAsync(coinId);
        }

        public async Task<IEnumerable<CoinGeckoTokenModel>> GetCoinsByNetwork(string network)
        {
            var tokens = await _tokenDataProvider.GetCoinsByNetwork(network);
            if (tokens != null)
            {
                await UpdateTokenDetails(tokens);
            }

            return tokens;
        }

        public Task<IEnumerable<TokenNetworkModel>> GetTokenNetworks()
        {
            return _tokenDataProvider.GetTokenNetworks();
        }

        public Task<IEnumerable<TokenDataModel>> GetTrendingTokens()
        {
            return _tokenDataProvider.GetTrendingTokens();
        }

        public async Task<IEnumerable<CoinGeckoTokenModel>> SearchCoin(string search)
        {
            var tokens = await _tokenDataProvider.SearchCoin(search);
            if (tokens != null)
            {
                await UpdateTokenDetails(tokens);
            }

            return tokens;
        }

        private async Task UpdateTokenDetails(IEnumerable<CoinGeckoTokenModel> tokens)
        {
            if (tokens != null)
            {
                foreach (var token in tokens)
                {
                    var tokenSocials = await _databaseService.GetTokenSocialsClaimByTokenAddress(token.Address);
                    token.Status = tokenSocials != null ? tokenSocials.Status : Models.SocialsClaimStatus.NotYetSet;
                }
            }
        }
    }
}
