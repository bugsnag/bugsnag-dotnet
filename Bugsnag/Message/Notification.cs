using Bugsnag.Message.Core;
using Bugsnag.Message.Event;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bugsnag.Message
{
    [DataContract]
    public class Notification
    {
        [DataMember(Name = "apiKey")]
        public string ApiKey { get; set; }

        [DataMember(Name = "notifier")]
        public NotifierInfo Notifier { get; set; }

        [DataMember(Name = "events")]
        public List<EventInfo> Events { get; set; }
    }
}
