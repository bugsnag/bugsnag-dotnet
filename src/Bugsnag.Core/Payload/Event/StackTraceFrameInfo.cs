using Newtonsoft.Json;

namespace Bugsnag.Core.Payload
{
    /// <summary>
    /// Contains information about a single frame of a stack trace
    /// </summary>
    public class StackTraceFrameInfo
    {
        /// <summary>
        /// Gets or sets the filename of the file the frame occurred (optional)
        /// </summary>
        [JsonProperty("file")]
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the line number of the frame
        /// </summary>
        [JsonProperty("lineNumber")]
        public int? LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the column number of the frame (optional)
        /// </summary>
        [JsonProperty("columnNumber")]
        public int? ColumnNumber { get; set; }

        /// <summary>
        /// Gets or sets the name of the method this stack frame is in
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the frame occurred in the users code. Allows filtering to only show frames
        /// that occur in the users code (optional, defaults to false)
        /// </summary>
        [JsonProperty("inProject")]
        public bool InProject { get; set; }
    }
}
