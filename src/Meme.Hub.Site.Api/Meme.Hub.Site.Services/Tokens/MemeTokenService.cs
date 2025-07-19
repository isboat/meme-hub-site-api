using Meme.Hub.Site.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Meme.Hub.Site.Services.Tokens
{
    public interface IMemeTokenService
    {
        Task<JsonObject> GetTrendingTokens();
    }
    public class MemeTokenService : IMemeTokenService
    {
        public async Task<JsonObject> GetTrendingTokens()
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://apiv2.moontok.io/api/common/trending");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<JsonObject>();
        }
    }
}
