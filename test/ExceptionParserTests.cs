using System;
using System.Diagnostics;
using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Test
{
    public class ExceptionParserTests
    {
        public const string TestApiKey = "ABCDEF1234567890ABCDEF1234567890";

        #region Test Classes and Methods

        public class TestClass
        {

        }

        public int Calculate(string string1, int integer2)
        {
            return 0;
        }

        public void Addition(long long1, long long2)
        {
            return;
        }

        public TestClass Subtraction(params uint[] unsigned)
        {
            return null;
        }

        public void Multiplication<T>(TestClass testClass)
        {
            return;
        }

        public void Nothing()
        {
            return;
        }

        public StackTrace CreateTrace()
        {
            return new StackTrace(true);
        }

        #endregion

        [Theory]
        [InlineData("Calculate", "Bugsnag.Test.ExceptionParserTests.Calculate(String string1, Int32 integer2)")]
        [InlineData("Addition", "Bugsnag.Test.ExceptionParserTests.Addition(Int64 long1, Int64 long2)")]
        [InlineData("Subtraction", "Bugsnag.Test.ExceptionParserTests.Subtraction(UInt32[] unsigned)")]
        [InlineData("Nothing", "Bugsnag.Test.ExceptionParserTests.Nothing()")]
        [InlineData("Multiplication", "Bugsnag.Test.ExceptionParserTests.Multiplication(TestClass testClass)")]
        public void GenerateMethodSignature_GeneratesSignaturesCorrectly(string methodName, string expSignature)
        {
            // Arrange
            var method = typeof(ExceptionParserTests).GetMethod(methodName);

            // Act
            var actSignature = ExceptionParser.GenerateMethodSignature(method);

            // Assert
            Assert.Equal(expSignature, actSignature);
        }

        [Fact]
        public void GenerateMethodSignature_NullMethodReturnsNullSignature()
        {
            // Act
            var actSignature = ExceptionParser.GenerateMethodSignature(null);

            // Assert
            Assert.Null(actSignature);
        }

        [Fact]
        public void GenerateExceptionInfo_NullIfNoStackTraceOnExceptionAndNoCallStack()
        {
            // Arrange
            var testConfig = new Configuration(TestApiKey);
            var exp = new SystemException("System Error");

            // Act
            StackTrace lastTrace = null;
            var actInfo = ExceptionParser.GenerateExceptionInfo(exp, null, null, testConfig, out lastTrace);

            // Assert
            Assert.Null(actInfo);
        }

        [Fact]
        public void GenerateExceptionInfo_NullIfExceptionIsNull()
        {
            // Arrange
            var testConfig = new Configuration(TestApiKey);

            // Act
            StackTrace lastTrace = null;
            var actInfoWithCall = ExceptionParser.GenerateExceptionInfo(null, new StackTrace(), null, testConfig, out lastTrace);
            lastTrace = null;
            var actInfoWithoutCall = ExceptionParser.GenerateExceptionInfo(null, null, null, testConfig, out lastTrace);

            // Assert
            Assert.Null(actInfoWithCall);
            Assert.Null(actInfoWithoutCall);
        }

        [Theory]
        [InlineData(true, false, false, false)]
        [InlineData(true, true, false, true)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, true, true)]
        [InlineData(false, false, false, false)]
        [InlineData(false, true, false, true)]
        [InlineData(false, false, true, true)]
        [InlineData(false, true, true, true)]
        public void GenerateExceptionInfo_GeneratesInfoWithExceptionStackTrace(
            bool useCallStack,
            bool autoInProject,
            bool projectNamespace,
            bool expInProject)
        {
            // Arrange
            var testConfig = new Configuration(TestApiKey);

            testConfig.AutoDetectInProject = autoInProject;
            if (projectNamespace)
                testConfig.ProjectNamespaces = new[] { "TestNamespace" };

            RankException testExp = null;
            try
            {
                TestNamespace.TestClass.ThrowException();
            }
            catch (Exception exp)
            {
                testExp = exp as RankException;
            }

            // Act
            StackTrace lastTrace = null;
            var actInfo = ExceptionParser.GenerateExceptionInfo(testExp, useCallStack ? new StackTrace() : null, null, testConfig, out lastTrace);

            // Assert
            Assert.NotNull(actInfo);
            Assert.Equal("RankException", actInfo.ExceptionClass);
            Assert.True(actInfo.Description.Contains(testExp.Message));
#if DEBUG
            // We can only be certain of this frame when in Debug mode. Optimisers are enabled in Release mode
            Assert.True(actInfo.StackTrace[0].File.EndsWith("ExceptionParserTests.cs"));
            Assert.Equal("TestNamespace.TestClass.ThrowException()", actInfo.StackTrace[0].Method);
            Assert.Equal(expInProject, actInfo.StackTrace[0].InProject);
#endif
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(true, true, true)]
        public void GenerateExceptionInfo_GeneratesInfoWithNoExceptionStackTraceButHasCallStack(
            bool autoInProject,
            bool projectNamespace,
            bool expInProject)
        {
            // Arrange
            var testConfig = new Configuration(TestApiKey);
            testConfig.AutoDetectInProject = autoInProject;
            if (projectNamespace)
                testConfig.ProjectNamespaces = new[] { "Bugsnag.Test.ExceptionParserTests" };

            var testExp = new RankException("Test Rank Exp");

            // Act
            StackTrace lastTrace = null;
            var actInfo = ExceptionParser.GenerateExceptionInfo(testExp, CreateTrace(), null, testConfig, out lastTrace);

            // Assert
            Assert.NotNull(actInfo);
            Assert.Equal("RankException", actInfo.ExceptionClass);
            Assert.True(actInfo.Description.Contains(testExp.Message));
            Assert.True(actInfo.StackTrace.Count > 0);
        }
    }
}

namespace TestNamespace
{
    public static class TestClass
    {
        public static void ThrowException()
        {
            throw new RankException("Test Rank Exp");
        }
    }
}
