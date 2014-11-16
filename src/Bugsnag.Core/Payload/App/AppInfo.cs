using Newtonsoft.Json;

namespace Bugsnag.Core.Payload
{
    public class AppInfo
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("releaseStage")]
        public string ReleaseStage { get; set; }

        [JsonProperty("appArchitecture")]
        public string AppArchitecture { get; set; }

        [JsonProperty("clrVersion")]
        public string ClrVersion { get; set; }
    }
}