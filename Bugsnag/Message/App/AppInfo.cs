using System.Runtime.Serialization;

namespace Bugsnag.Message.App
{
    [DataContract]
    public class AppInfo
    {
        [DataMember(Name = "version", IsRequired = false, EmitDefaultValue = false)]
        public string Version { get; set; }

        [DataMember(Name = "releaseStage", IsRequired = false, EmitDefaultValue = false)]
        public string ReleaseStage { get; set; }
    }
}