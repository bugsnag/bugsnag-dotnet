using Newtonsoft.Json;

namespace Bugsnag.Core.Payload
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
