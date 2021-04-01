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
        public string AiKey { get; private set; }
        [JsonProperty("dev")]
        public bool Dev { get; private set; }
    }
}
