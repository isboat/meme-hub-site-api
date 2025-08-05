namespace Meme.Hub.Site.Services.Providers.Tokens
{
    public class CoinGeckoService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.coingecko.com/api/v3";
        private const string apiKeyQs = "x_cg_demo_api_key=CG-gphvLEo5jN2VdVLydwYL9sgg";

        public CoinGeckoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetCoinDataByIdAsync(string coinId)
        {
            if (string.IsNullOrWhiteSpace(coinId))
                throw new ArgumentException("Coin ID cannot be null or empty.", nameof(coinId));

            var url = $"{BaseUrl}/coins/{coinId}";
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
