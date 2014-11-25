using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bugsnag.Core.Payload
{
    /// <summary>
    /// Represents a single notification object to send to Bugsnag
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Gets or sets the API key associated with the Bugsnag account to send the notification to
        /// </summary>
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the information about the notifier used 
        /// </summary>
        [JsonProperty("notifier")]
        public NotifierInfo Notifier { get; set; }

        /// <summary>
        /// Gets or sets the list of events to send in the notification (normally one event)
        /// </summary>
        [JsonProperty("events")]
        public List<EventInfo> Events { get; set; }
    }
}
