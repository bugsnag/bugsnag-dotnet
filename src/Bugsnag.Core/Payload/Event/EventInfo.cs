using System.Collections.Generic;
using Newtonsoft.Json;
using TabData = System.Collections.Generic.Dictionary<string, object>;

namespace Bugsnag.Payload
{
    /// <summary>
    /// Contains information about a single event contained in the notification 
    /// </summary>
    internal class EventInfo
    {
        /// <summary>
        /// Gets the current version of the payload describing the event
        /// </summary>
        [JsonProperty("payloadVersion")]
        public int PayloadVersion
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets or sets the user information associated with the event
        /// </summary>
        [JsonProperty("user")]
        public UserInfo User { get; set; }

        /// <summary>
        /// Gets or sets the application information associated with the event
        /// </summary>
        [JsonProperty("app")]
        public AppInfo App { get; set; }

        /// <summary>
        /// Gets or sets the information about the application state at the time of the event (optional)
        /// </summary>
        [JsonProperty("appState")]
        public AppStateInfo AppState { get; set; }

        /// <summary>
        /// Gets or sets the information about the device
        /// </summary>
        [JsonProperty("device")]
        public DeviceInfo Device { get; set; }

        /// <summary>
        /// Gets or sets information about the device state at the time of the event (optional)
        /// </summary>
        [JsonProperty("deviceState")]
        public DeviceStateInfo DeviceState { get; set; }

        /// <summary>
        /// Gets or sets the current context at the time of the event (optional)
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets the severity of the event (optional)
        /// </summary>
        [JsonProperty("severity")]
        public Severity Severity { get; set; }

        /// <summary>
        /// Gets or sets the grouping hash. All errors with the same grouping Hash will be grouped together (optional)
        /// </summary>
        [JsonProperty("groupingHash")]
        public string GroupingHash { get; set; }

        /// <summary>
        /// Gets or sets the exception (or exceptions if there are nested exceptions) in the event 
        /// </summary>
        [JsonProperty("exceptions")]
        public List<ExceptionInfo> Exceptions { get; set; }

        /// <summary>
        /// Gets or sets the state of the other managed threads at the time of the event (optional)
        /// </summary>
        [JsonProperty("threads")]
        public List<ThreadInfo> Threads { get; set; }

        /// <summary>
        /// Gets or sets any additional data to send with the event (optional)
        /// </summary>
        [JsonProperty("metaData")]
        public Dictionary<string, TabData> Metadata { get; set; }
    }
}
