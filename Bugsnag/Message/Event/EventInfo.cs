using Bugsnag.Message.App;
using Bugsnag.Message.Core;
using Bugsnag.Message.Device;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bugsnag.Message.Event
{
    [DataContract]
    public class EventInfo
    {
        [DataMember(Name = "user", IsRequired = false, EmitDefaultValue = false)]
        public UserInfo User { get; set; }

        [DataMember(Name = "app", IsRequired = false, EmitDefaultValue = false)]
        public AppInfo App { get; set; }

        [DataMember(Name = "appState", IsRequired = false, EmitDefaultValue = false)]
        public AppStateInfo AppState { get; set; }

        [DataMember(Name = "device", IsRequired = false, EmitDefaultValue = false)]
        public DeviceInfo Device { get; set; }

        [DataMember(Name = "deviceState", IsRequired = false, EmitDefaultValue = false)]
        public DeviceStateInfo DeviceState { get; set; }

        [DataMember(Name = "context", IsRequired = false, EmitDefaultValue = false)]
        public string Context { get; set; }

        [DataMember(Name = "severity", IsRequired = false, EmitDefaultValue = false)]
        public string Severity { get; set; }

        [DataMember(Name = "payloadVersion")]
        public const int PayloadVersion = 2;

        [DataMember(Name = "groupingHash", IsRequired = false, EmitDefaultValue = false)]
        public string GroupingHash { get; set; }

        [DataMember(Name = "exceptions")]
        public List<ExceptionInfo> Exceptions { get; set; }

        [DataMember(Name = "threads", IsRequired = false, EmitDefaultValue = false)]
        public List<ThreadInfo> Threads { get; set; }
    }
}
