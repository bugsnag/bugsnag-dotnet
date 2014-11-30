using System;
using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Core.Test
{
    public class EventTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void Constructor_CallStackIsRecordedIfStackTraceNotPresent(bool? isRunningFlag)
        {
            // Arrange
            var noTraceException = new AccessViolationException("Manually Created, no trace");

            // Act
            Event actEvent = null;
            if (isRunningFlag.HasValue)
                actEvent = new Event(noTraceException, isRunningFlag.Value);
            else
                actEvent = new Event(noTraceException);

            // Assert
            Assert.NotNull(actEvent.CallTrace);
            Assert.Equal("Constructor_CallStackIsRecordedIfStackTraceNotPresent", actEvent.CallTrace.GetFrame(0).GetMethod().Name);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void Constructor_CallStackIsRecordedWhenThereIsNoException(bool? isRunningFlag)
        {
            // Act
            Event actEvent = null;
            if (isRunningFlag.HasValue)
                actEvent = new Event(null, isRunningFlag.Value);
            else
                actEvent = new Event(null);

            // Assert
            Assert.NotNull(actEvent.CallTrace);
            Assert.Equal("Constructor_CallStackIsRecordedWhenThereIsNoException", actEvent.CallTrace.GetFrame(0).GetMethod().Name);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void Constructor_CallStackIsNotRecordedIfStackTracePresent(bool? isRunningFlag)
        {
            // Arrange
            Exception traceException;
            try
            {
                throw new OutOfMemoryException("With trace");
            }
            catch (Exception exp)
            {
                traceException = exp;
            }

            // Act
            Event actEvent = null;
            if (isRunningFlag.HasValue)
                actEvent = new Event(traceException, isRunningFlag.Value);
            else
                actEvent = new Event(traceException);

            // Assert
            Assert.Null(actEvent.CallTrace);
        }
    }
}
