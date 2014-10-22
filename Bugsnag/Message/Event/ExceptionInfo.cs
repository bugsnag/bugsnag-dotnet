using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bugsnag.Message.Event
{
    [DataContract]
    public class ExceptionInfo
    {
        [DataMember(Name = "errorClass")]
        public string ExceptionClass { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "stacktrace")]
        public List<StackTraceFrameInfo> StackTrace { get; set; }
    }
}
