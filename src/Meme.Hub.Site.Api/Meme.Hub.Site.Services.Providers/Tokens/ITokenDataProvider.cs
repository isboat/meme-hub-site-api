using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models.MemeTokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Meme.Hub.Site.Services.Providers.Tokens
{
    public interface ITokenDataProvider
    {
        Task<IEnumerable<TokenChains>> GetTokenNetworks();

        Task<JsonObject> GetCoinDataByIdAsync(string coinId);

        Task<IEnumerable<TokenDataModel>> GetTrendingTokens();

        Task<IEnumerable<TokenDetailsDto>> GetCoinsByNetwork(string network);
    }

    public interface ICoinGeckoProvider
    {

        Task<List<TokenNetworkModel>> GetTokenNetworks();

        Task<List<CoinGeckoTokenModel>> GetTokensByNetworkId(string networkId);

        Task<string> GetCoinDataByIdAsync(string coinId);
    }
}
