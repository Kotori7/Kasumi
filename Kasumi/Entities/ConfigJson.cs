using Newtonsoft.Json;

namespace Kasumi.Entities
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
        [JsonProperty("osu_key")]
        public string OsuKey { get; private set; }
        [JsonProperty("ai_key")]
        public string AIKey { get; set; }
    }
}
