using Newtonsoft.Json;

namespace Bugsnag.Message.Event
{
    public class StackTraceFrameInfo
    {
        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("lineNumber")]
        public int? LineNumber { get; set; }

        [JsonProperty("columnNumber")]
        public int? ColumnNumber { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("inProject")]
        public bool InProject { get; set; }
    }
}
