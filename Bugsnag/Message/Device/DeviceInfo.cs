using System.Runtime.Serialization;

namespace Bugsnag.Message.Device
{
    [DataContract]
    public class DeviceInfo
    {
        [DataMember(Name = "osVersion", IsRequired = false, EmitDefaultValue = false)]
        public string OsVersion { get; set; }

        [DataMember(Name = "hostname", IsRequired = false, EmitDefaultValue = false)]
        public string Hostname { get; set; }
    }
}
