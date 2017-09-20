using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;

namespace Bugsnag
{
    /// <summary>
    /// The reason for assigning a particular severity
    /// </summary>
    public enum SeverityReason
    {
        [Description("unhandledException")]
        UnhandledException,
        [Description("handledException")]
        HandledException,
        [Description("userSpecifiedSeverity")]
        UserSpecified,
        [Description("userCallbackSetSeverity")]
        CallbackSpecified,
        [Description("WPF")]
        UnhandledExceptionMiddlewareWPF,
        [Description("WebAPI")]
        UnhandledExceptionMiddlewareWebAPI,
        [Description("ASP.NET")]
        UnhandledExceptionMiddlewareWebMVC,
    }

    public static class SeverityReasonExtensions
    {
        const string KeyType = "type";
        const string KeyAttrs = "attributes";
        const string KeyFramework = "framework";
        const string MiddlewareReasonValue = "unhandledExceptionMiddleware";

        public static string ToDescription(this SeverityReason reason)
        {
            FieldInfo info = reason.GetType().GetField(reason.ToString());
            var attrs = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attrs[0].Description;
        }

        public static bool Unhandled(this SeverityReason reason)
        {
            switch (reason)
            {
                case SeverityReason.UnhandledException:
                case SeverityReason.UnhandledExceptionMiddlewareWebAPI:
                case SeverityReason.UnhandledExceptionMiddlewareWebMVC:
                case SeverityReason.UnhandledExceptionMiddlewareWPF:
                    return true;
                default:
                    return false;
            }
        }

        public static Severity DefaultSeverity(this SeverityReason reason)
        {
            if (reason.Unhandled())
                return Severity.Error;
            return Severity.Warning;
        }

        public static Dictionary<string, object> Serialize(this SeverityReason reason)
        {
            string description = reason.ToDescription();
            Dictionary<string, object> reasonAttrs = new Dictionary<string, object>();
            switch (reason)
            {
                case SeverityReason.UnhandledExceptionMiddlewareWebAPI:
                case SeverityReason.UnhandledExceptionMiddlewareWPF:
                case SeverityReason.UnhandledExceptionMiddlewareWebMVC:
                    Dictionary<string, string> detailAttrs = new Dictionary<string, string>
                    {
                        { KeyFramework, description }
                    };
                    reasonAttrs.Add(KeyType, MiddlewareReasonValue);
                    reasonAttrs.Add(KeyAttrs, detailAttrs);
                    break;
                default:
                    reasonAttrs.Add(KeyType, description);
                    break;
            }

            return reasonAttrs;
        }
    }
}
