using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bugsnag.Payload.Event
{
    public class ExceptionInfo
    {
        [JsonProperty("errorClass")]
        public string ExceptionClass { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("stacktrace")]
        public List<StackTraceFrameInfo> StackTrace { get; set; }
    }
}
