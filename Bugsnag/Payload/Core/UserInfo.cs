using Newtonsoft.Json;

namespace Bugsnag.Payload.Core
{
    public class UserInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("loggedOnUser")]
        public string LoggedOnUser { get; set; }
    }
}