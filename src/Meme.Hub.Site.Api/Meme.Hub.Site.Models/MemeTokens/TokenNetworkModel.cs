using Newtonsoft.Json;

namespace Meme.Hub.Site.Models.MemeTokens
{
    public class TokenNetworkModel
    {
        public string Id { get; set; }

        [JsonProperty("chain_identifier")]
        public string ChainIdentifier { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        [JsonProperty("native_coin_id")]
        public string NativeCoinId { get; set; }
        public TokenImageModel Image { get; set; }
    }

    public class TokenImageModel
    {
        public string Thumb { get; set; }
        public string Small { get; set; }
        public string Large { get; set; }
    }
}