﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bugsnag
{
    /// <summary>
    /// Defines the levels of severity an event can be
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Severity
    {
        /// <summary>
        /// Informational event
        /// </summary>
        [EnumMember(Value = "info")]
        Info,

        /// <summary>
        /// Error event (typically used for crashing exceptions)
        /// </summary>
        [EnumMember(Value = "error")]
        Error,

        /// <summary>
        /// Warning event (typically used for caught exceptions)
        /// </summary>
        [EnumMember(Value = "warning")]
        Warning
    }

    /// <summary>
    /// Defines the available types of clients for sending errors
    /// </summary>
    public enum ClientTypes
    {
        Standard,
        WebApi,
        WebMvc,
        Wpf
    }
}
