using Bugsnag.Event;
using Bugsnag.Message.Core;
using Bugsnag.Message.Event;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bugsnag.Message
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
