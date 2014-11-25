using Newtonsoft.Json;

namespace Bugsnag.Core.Payload
{
    /// <summary>
    /// Contains information about the current user of the application
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Gets or sets the unique identifier attached to the event (optional)
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the user (optional)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user (optional)
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user currently logged on to the system, if applicable (optional)
        /// </summary>
        [JsonProperty("loggedOnUser")]
        public string LoggedOnUser { get; set; }
    }
}