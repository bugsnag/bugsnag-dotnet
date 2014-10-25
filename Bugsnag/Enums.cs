using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Bugsnag
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Severity
    {
        [EnumMember(Value = "info")]
        Info,
        [EnumMember(Value = "error")]
        Error,
        [EnumMember(Value = "warning")]
        Warning
    }
}
