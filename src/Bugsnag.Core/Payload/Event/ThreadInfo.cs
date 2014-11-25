using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bugsnag.Core.Payload
{
    /// <summary>
    /// Contains information about a single managed thread
    /// </summary>
    public class ThreadInfo
    {
        /// <summary>
        /// Gets or sets a unique identifier for the thread
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the thread
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current stack trace of the thread at the time of the event
        /// </summary>
        [JsonProperty("stacktrace")]
        public List<StackTraceFrameInfo> StackTrace { get; set; }
    }
}