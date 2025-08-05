using Meme.Domain.Models.TokenModels;
using Meme.Hub.Site.Models.MemeTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Meme.Hub.Site.Services.Providers.Tokens
{
    public class MoonTokProvider : ITokenDataProvider
    {
        private readonly HttpClient _httpClient;
        //private const string BaseUrl = "https://apiv2.moontok.io/api";

        public MoonTokProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<JsonObject> GetCoinDataByIdAsync(string coinId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TokenDetailsDto>> GetCoinsByNetwork(string network) // Example: "solana"
        {
            var url = $"api/tokens?type=CRYPTOCURRENCIES&sortBy=rank&sortDir=DESC&chain={network}&page=1&pageSize=100";
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return []; // Return empty data if not found
            }
            response.EnsureSuccessStatusCode();

            var jsonObject = await response.Content.ReadFromJsonAsync<JsonObject>();
            if (jsonObject == null || !jsonObject.ContainsKey("data"))
            {
                return []; // Return empty data if no data found
            }
            var tokens = jsonObject["data"]?.AsArray();
            if (tokens == null)
            {
                return []; // Return empty data if no tokens found
            }
            // Convert JsonArray to IEnumerable<TokenDetailsDto>
            return tokens.Select(token => new TokenDetailsDto
            {
                Id = token["id"]?.ToString(),
                Name = token["name"]?.ToString(),
                Slug = token["slug"]?.ToString(),
                Symbol = token["symbol"]?.ToString(),
                CreatedAt = DateTime.Parse(token["createdAt"]?.ToString() ?? DateTime.MinValue.ToString()),
                UpdatedAt = DateTime.Parse(token["updatedAt"]?.ToString() ?? DateTime.MinValue.ToString()),
                DeletedAt = token["deletedAt"]?.GetValue<DateTime?>(),
                LogoUrl = token["logoUrl"]?.ToString(),
                Price = token["price"]?.GetValue<decimal>() ?? 0,
                Marketcap = token["marketcap"]?.GetValue<decimal>() ?? 0,
                Liquidity = token["liquidity"]?.GetValue<decimal>() ?? 0,
                PriceChangeH1 = token["priceChangeH1"]?.GetValue<decimal>() ?? 0,
                PriceChangeH6 = token["priceChangeH6"]?.GetValue<decimal>() ?? 0,
                PriceChangeH24 = token["priceChangeH24"]?.GetValue<decimal>() ?? 0,
                ListedAt = DateTime.Parse(token["listedAt"]?.ToString() ?? DateTime.MinValue.ToString()),
                IsExpressListing = token["isExpressListing"]?.GetValue<bool>() ?? false,
                Status = token["status"]?.GetValue<int>() ?? 0,
                LaunchedAt = DateTime.Parse(token["launchedAt"]?.ToString() ?? DateTime.MinValue.ToString()),
                CreatedBy = token["createdBy"]?.ToString(),
                Addresses = token["addresses"]?.AsArray()?.Select(address => new TokenAddressDto
                {
                    Id = address["id"]?.ToString(),
                    TokenAddress = address["tokenAddress"]?.ToString(),
                    PairAddress = address["pairAddress"]?.ToString(),
                    Chain = address["chain"] != null ? new ChainDto
                    {
                        Id = address["chain"]["id"]?.ToString(),
                        Name = address["chain"]["name"]?.ToString()
                    } : null
                }).ToList() ?? []
            });
        }

        public async Task<IEnumerable<TokenChains>> GetTokenNetworks()
        {
            var url = $"api/chains";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonObject>();

            var chains = json?["data"]?.AsArray();
            if (chains == null)
            {
                return [];
            }

            // Convert JsonArray to IEnumerable<TokenChains>
            return chains.Select(chain => new TokenChains
            {
                Id = chain["id"]?.ToString(),
                ChainId = chain["chainId"]?.ToString(),
                Name = chain["name"]?.ToString(),
                Abbr = chain["abbr"]?.ToString(),
                Slug = chain["slug"]?.ToString(),
                CurrencySymbol = chain["currencySymbol"]?.ToString(),
                ExplorerUrl = chain["explorerUrl"]?.ToString(),
                DextoolsIdentifier = chain["dextoolsIdentifier"]?.ToString(),
                GeckoterminalIdentifier = chain["geckoterminalIdentifier"]?.ToString(),
                PayCurrency = chain["payCurrency"]?.ToString(),
                Emoji = chain["emoji"]?.ToString(),
                LogoUrl = chain["logoUrl"]?.ToString(),
                PriceUsd = chain["priceUsd"]?.GetValue<decimal>() ?? 0,
                PriceUpdatedAt = DateTime.Parse(chain["priceUpdatedAt"]?.ToString() ?? DateTime.MinValue.ToString()),
                Ranking = chain["ranking"]?.GetValue<int>() ?? 0,
                CreatedAt = DateTime.Parse(chain["createdAt"]?.ToString() ?? DateTime.MinValue.ToString()),
                UpdatedAt = DateTime.Parse(chain["updatedAt"]?.ToString() ?? DateTime.MinValue.ToString())
            });
        }

        public async Task<IEnumerable<TokenDataModel>> GetTrendingTokens()
        {
            var url = $"api/common/trending";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonObject>();
            var tokens = json?["tokens"]?.AsArray();
            if (tokens == null)
            {
                return Enumerable.Empty<TokenDataModel>();
            }
            // Convert JsonArray to IEnumerable<TokenDataModel>
            return tokens.Select(token => new TokenDataModel
            {
                Id = token["id"]?.ToString(),
                Name = token["name"]?.ToString(),
                Symbol = token["symbol"]?.ToString(),
                //ImageUrl = token["imageUrl"]?.ToString(),
                //Network = token["chain"]?.ToString(),
                //Price = token["price"]?.GetValue<decimal>() ?? 0,
                //MarketCap = token["marketCap"]?.GetValue<decimal>() ?? 0,
                //Volume24h = token["volume24h"]?.GetValue<decimal>() ?? 0,
                //Change24h = token["change24h"]?.GetValue<decimal>() ?? 0
            });
        }
    }
}
