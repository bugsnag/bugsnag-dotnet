using Newtonsoft.Json;

namespace Bugsnag.Core.Payload
{
    /// <summary>
    /// Contains information about the notifier used to send notifications
    /// </summary>
    public class NotifierInfo
    {
        /// <summary>
        /// Gets or sets the name of the notifier
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version of the notifier
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the URL associated with the notifier
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
