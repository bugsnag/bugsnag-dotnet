using System.Runtime.Serialization;

namespace Bugsnag.Message.Core
{
    [DataContract]
    public class NotifierInfo
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
