using Meme.Hub.Site.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meme.Hub.Site.Api.Controllers
{
    [Route("api/[controller]")]
    public class TwitterController : Controller
    {
        private readonly TwitterOAuthSettings twitterOAuthSettings;
        public TwitterController(IOptions<TwitterOAuthSettings> settings)
        {
            twitterOAuthSettings = settings.Value;
        }

        [HttpPost("oauth2/token")]
        public async Task<string> OAuth2Token(OAuth2Token oauth2Token)
        {
            using var httpClient = new HttpClient();


            // Twitter OAuth2 token endpoint
            var tokenUrl = "https://api.twitter.com/2/oauth2/token";

            var authHeaderValue = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{twitterOAuthSettings.ClientId}:{twitterOAuthSettings.ClientSecret}")
            );
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

            // Prepare request body
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", twitterOAuthSettings.ClientId),
                new KeyValuePair<string, string>("client_secret", twitterOAuthSettings.ClientSecret),
                new KeyValuePair<string, string>("code", oauth2Token.Code),
                new KeyValuePair<string, string>("redirect_uri", oauth2Token.RedirectUri),
                new KeyValuePair<string, string>("code_verifier", oauth2Token.CodeVerifier)
            });

            // Send POST request
            var response = await httpClient.PostAsync(tokenUrl, requestBody);

            // Read response
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        [HttpGet("users/me/{token}")]
        public async Task<string> UsersMe(string token)
        {
            using var httpClient = new HttpClient();

            // Twitter endpoint
            var tokenUrl = "https://api.twitter.com/2/users/me";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send Get request
            var response = await httpClient.GetAsync(tokenUrl);

            // Read response
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        // Backend should perform the call to Twitter API using the stored token.
        [HttpPost("tweets/{token}")]
        public async Task<ActionResult<string>> PostTweet([FromRoute] string token, [FromBody] PostTweet postTweet)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(postTweet.Text))
            {
                return BadRequest(ModelState); // Return validation errors
            }

            using var httpClient = new HttpClient();

            // Twitter endpoint
            var url = "https://api.twitter.com/2/tweets";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(postTweet), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);

            return await response.Content.ReadAsStringAsync(); ;
        }
    }

    public class OAuth2Token
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }

        public required string CodeVerifier { get; set; }
    }

    public class PostTweet
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
