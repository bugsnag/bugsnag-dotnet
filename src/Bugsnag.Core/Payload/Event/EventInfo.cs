using Newtonsoft.Json;
using System.Collections.Generic;
using TabData = System.Collections.Generic.Dictionary<string, object>;

namespace Bugsnag.Core.Payload
{
    public class EventInfo
    {
        [JsonProperty("user")]
        public UserInfo User { get; set; }

        [JsonProperty("app")]
        public AppInfo App { get; set; }

        [JsonProperty("appState")]
        public AppStateInfo AppState { get; set; }

        [JsonProperty("device")]
        public DeviceInfo Device { get; set; }

        [JsonProperty("deviceState")]
        public DeviceStateInfo DeviceState { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

        [JsonProperty("severity")]
        public Severity Severity { get; set; }

        [JsonProperty("payloadVersion")]
        public int PayloadVersion { get { return 2; } }

        [JsonProperty("groupingHash")]
        public string GroupingHash { get; set; }

        [JsonProperty("exceptions")]
        public List<ExceptionInfo> Exceptions { get; set; }

        [JsonProperty("threads")]
        public List<ThreadInfo> Threads { get; set; }

        [JsonProperty("metaData")]
        public Dictionary<string, TabData> Metadata { get; set; }
    }
}
