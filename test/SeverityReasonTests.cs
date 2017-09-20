using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Test
{
    public class SeverityReasonTests
    {
        [Fact]
        public void TestUnhandled()
        {
            Assert.Equal(false, SeverityReason.UserSpecified.Unhandled());
            Assert.Equal(false, SeverityReason.CallbackSpecified.Unhandled());
            Assert.Equal(false, SeverityReason.HandledException.Unhandled());
            Assert.Equal(true, SeverityReason.UnhandledException.Unhandled());
            Assert.Equal(true, SeverityReason.UnhandledExceptionMiddlewareWPF.Unhandled());
            Assert.Equal(true, SeverityReason.UnhandledExceptionMiddlewareWebAPI.Unhandled());
            Assert.Equal(true, SeverityReason.UnhandledExceptionMiddlewareWebMVC.Unhandled());
        }

        [Fact]
        public void TestDefaultSeverity()
        {
            Assert.Equal(Severity.Warning, SeverityReason.UserSpecified.DefaultSeverity());
            Assert.Equal(Severity.Warning, SeverityReason.CallbackSpecified.DefaultSeverity());
            Assert.Equal(Severity.Warning, SeverityReason.HandledException.DefaultSeverity());
            Assert.Equal(Severity.Error, SeverityReason.UnhandledException.DefaultSeverity());
            Assert.Equal(Severity.Error, SeverityReason.UnhandledExceptionMiddlewareWPF.DefaultSeverity());
            Assert.Equal(Severity.Error, SeverityReason.UnhandledExceptionMiddlewareWebAPI.DefaultSeverity());
            Assert.Equal(Severity.Error, SeverityReason.UnhandledExceptionMiddlewareWebMVC.DefaultSeverity());
        }

        [Fact]
        public void TestSerializeUserSpecified()
        {
            Dictionary<string, object> data = SeverityReason.UserSpecified.Serialize();
            Assert.Equal(1, data.Keys.Count);
            Assert.Equal("userSpecifiedSeverity", data["type"]);
        }

        [Fact]
        public void TestSerializeUnhandledExceptionMiddlewareWebMVC()
        {
            Dictionary<string, object> data = SeverityReason.UnhandledExceptionMiddlewareWebMVC.Serialize();
            Assert.Equal(2, data.Keys.Count);
            Assert.Equal("unhandledExceptionMiddleware", data["type"]);

            var attrs = (Dictionary<string, string>)data["attributes"];
            Assert.Equal(1, attrs.Keys.Count);
            Assert.Equal("ASP.NET", attrs["framework"]);
        }

        [Fact]
        public void TestSerializeUnhandledExceptionMiddlewareWebAPI()
        {
            Dictionary<string, object> data = SeverityReason.UnhandledExceptionMiddlewareWebAPI.Serialize();
            Assert.Equal(2, data.Keys.Count);
            Assert.Equal("unhandledExceptionMiddleware", data["type"]);

            var attrs = (Dictionary<string, string>)data["attributes"];
            Assert.Equal(1, attrs.Keys.Count);
            Assert.Equal("WebAPI", attrs["framework"]);
        }

        [Fact]
        public void TestSerializeUnhandledExceptionMiddlewareWPF()
        {
            Dictionary<string, object> data = SeverityReason.UnhandledExceptionMiddlewareWPF.Serialize();
            Assert.Equal(2, data.Keys.Count);
            Assert.Equal("unhandledExceptionMiddleware", data["type"]);

            var attrs = (Dictionary<string, string>)data["attributes"];
            Assert.Equal(1, attrs.Keys.Count);
            Assert.Equal("WPF", attrs["framework"]);
        }

        [Fact]
        public void TestSerializeHandledException()
        {
            Dictionary<string, object> data = SeverityReason.HandledException.Serialize();
            Assert.Equal(1, data.Keys.Count);
            Assert.Equal("handledException", data["type"]);
        }

        [Fact]
        public void TestSerializeUnhandledException()
        {
            Dictionary<string, object> data = SeverityReason.UnhandledException.Serialize();
            Assert.Equal(1, data.Keys.Count);
            Assert.Equal("unhandledException", data["type"]);
        }
    }
}
