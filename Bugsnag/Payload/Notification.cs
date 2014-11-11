using Bugsnag.Payload.Core;
using Bugsnag.Payload.Event;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bugsnag.Payload
{
    public class Notification
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("notifier")]
        public NotifierInfo Notifier { get; set; }

        [JsonProperty("events")]
        public List<EventInfo> Events { get; set; }
    }
}
