using Newtonsoft.Json;

namespace Bugsnag.Payload.Core
{
    public class NotifierInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
