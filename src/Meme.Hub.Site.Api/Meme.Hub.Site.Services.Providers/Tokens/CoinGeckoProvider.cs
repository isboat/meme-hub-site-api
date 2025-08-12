using Meme.Hub.Site.Models.MemeTokens;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Meme.Hub.Site.Services.Providers.Tokens
{
    public class CoinGeckoProvider: ICoinGeckoProvider
    {
        private readonly HttpClient _httpClient;
        private const string PathPrefix = "/api/v3";

        public CoinGeckoProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TokenNetworkModel>> GetTokenNetworks()
        {
            var url = GetPath($"/asset_platforms");
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed: {response.StatusCode} - {error}");
            }

            var str = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<TokenNetworkModel>>(str);
            return result ?? [];
        }

        private static string? GetPath(string path) => $"{PathPrefix}{path}";

        public async Task<List<CoinGeckoTokenModel>> GetTokensByNetworkId(string networkId)
        {
            var url = GetPath($"/token_lists/{networkId}/all.json");
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed: {response.StatusCode} - {error}");
            }

            var json = await response.Content.ReadFromJsonAsync<JsonObject>();

            var tokens = json?["tokens"]?.AsArray();
            if (tokens == null)
            {
                return [];
            }

            return tokens.Select(token => new CoinGeckoTokenModel
            {
                ChainId = token["chain_id"]?.GetValue<string>(),
                Address = token["address"]?.GetValue<string>(),
                Name = token["name"]?.GetValue<string>(),
                Symbol = token["symbol"]?.GetValue<string>(),
                Decimals = token["decimals"]?.GetValue<int>() ?? 0,
                LogoURI = token["logo_uri"]?.GetValue<string>()
            }).ToList();
        }
        public async Task<string> GetCoinDataByIdAsync(string coinId)
        {
            if (string.IsNullOrWhiteSpace(coinId))
                throw new ArgumentException("Coin ID cannot be null or empty.", nameof(coinId));

            var url = GetPath($"/coins/{coinId}");
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed: {response.StatusCode} - {error}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
