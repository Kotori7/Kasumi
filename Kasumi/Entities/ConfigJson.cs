using Newtonsoft.Json;

namespace Kasumi.Entities
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }

        [JsonProperty("dev")]
        public bool Dev { get; private set; }
        
        [JsonProperty("new_relic_account_id")]
        public string NewRelicID { get; private set; }
        
        [JsonProperty("new_relic_license_key")]
        public string NewRelicKey { get; private set; }
    }
}
