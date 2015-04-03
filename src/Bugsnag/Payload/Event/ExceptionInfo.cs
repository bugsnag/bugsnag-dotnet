using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bugsnag.Payload
{
    /// <summary>
    /// Contains information about a single exception
    /// </summary>
    internal class ExceptionInfo
    {
        /// <summary>
        /// Gets or sets the name of the exception class
        /// </summary>
        [JsonProperty("errorClass")]
        public string ExceptionClass { get; set; }

        /// <summary>
        /// Gets or sets the descriptive message associated with an exception
        /// </summary>
        [JsonProperty("message")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the stack trace of the exception
        /// </summary>
        [JsonProperty("stacktrace")]
        public List<StackTraceFrameInfo> StackTrace { get; set; }
    }
}
