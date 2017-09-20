using System;
using System.Text;
using System.Collections.Generic;
using Xunit;

namespace Bugsnag.Test
{
    public class HandledStateTests
    {
        [Fact]
        public void TestDefaultSeverity()
        {
            Assert.Equal(new HandledState(SeverityReason.CallbackSpecified).OriginalSeverity, Severity.Warning);
            Assert.Equal(new HandledState(SeverityReason.HandledException).OriginalSeverity, Severity.Warning);
            Assert.Equal(new HandledState(SeverityReason.UserSpecified).OriginalSeverity, Severity.Warning);
            Assert.Equal(new HandledState(SeverityReason.UnhandledException).OriginalSeverity, Severity.Error);
            Assert.Equal(new HandledState(SeverityReason.UnhandledExceptionMiddlewareWebAPI).OriginalSeverity, Severity.Error);
            Assert.Equal(new HandledState(SeverityReason.UnhandledExceptionMiddlewareWPF).OriginalSeverity, Severity.Error);
            Assert.Equal(new HandledState(SeverityReason.UnhandledExceptionMiddlewareWebMVC).OriginalSeverity, Severity.Error);

            Assert.Equal(new HandledState(SeverityReason.CallbackSpecified).CurrentSeverity, Severity.Warning);
            Assert.Equal(new HandledState(SeverityReason.HandledException).CurrentSeverity, Severity.Warning);
            Assert.Equal(new HandledState(SeverityReason.UserSpecified).CurrentSeverity, Severity.Warning);
            Assert.Equal(new HandledState(SeverityReason.UnhandledException).CurrentSeverity, Severity.Error);
            Assert.Equal(new HandledState(SeverityReason.UnhandledExceptionMiddlewareWebAPI).CurrentSeverity, Severity.Error);
            Assert.Equal(new HandledState(SeverityReason.UnhandledExceptionMiddlewareWPF).CurrentSeverity, Severity.Error);
            Assert.Equal(new HandledState(SeverityReason.UnhandledExceptionMiddlewareWebMVC).CurrentSeverity, Severity.Error);
        }

        public void TestSetSeverity()
        {
            Assert.Equal(new HandledState(SeverityReason.CallbackSpecified, Severity.Info).CurrentSeverity, Severity.Info);
            Assert.Equal(new HandledState(SeverityReason.UserSpecified, Severity.Info).CurrentSeverity, Severity.Info);
        }
    }
}
