using Bugsnag.Payload.App;
using Bugsnag.Payload.Core;
using Bugsnag.Payload.Device;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bugsnag.Payload.Event
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
        public int PayloadVersion {get {return 2;}}

        [JsonProperty("groupingHash")]
        public string GroupingHash { get; set; }

        [JsonProperty("exceptions")]
        public List<ExceptionInfo> Exceptions { get; set; }

        [JsonProperty("threads")]
        public List<ThreadInfo> Threads { get; set; }

        [JsonProperty("metaData")]
        public Dictionary<string, Dictionary<string, object>> MetaData { get; set; }
    }
}
