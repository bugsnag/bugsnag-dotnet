using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bugsnag.Core.Payload.Event
{
    public class ThreadInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("stacktrace")]
        public List<StackTraceFrameInfo> StackTrace { get; set; }
    }
}