using System.Runtime.Serialization;

namespace Bugsnag.Message.Event
{
    [DataContract]
    public class StackTraceFrameInfo
    {
        [DataMember(Name = "file")]
        public string File { get; set; }

        [DataMember(Name = "lineNumber")]
        public int LineNumber { get; set; }

        [DataMember(Name = "columnNumber", IsRequired = false, EmitDefaultValue = false)]
        public int ColumnNumber { get; set; }

        [DataMember(Name = "method")]
        public string Method { get; set; }

        [DataMember(Name = "inProject", IsRequired = false, EmitDefaultValue = false)]
        public bool InProject { get; set; }
    }
}
