using System;
using System.Linq;
using System.Threading.Tasks;
using Bugsnag.Clients;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Bugsnag.Test.FunctionalTests
{
    public class ExceptionTypeTests
    {
        [Fact]
        public void CheckInnerExceptionsAreNotified()
        {
            // Arrange
            var client = new BaseClient(StaticData.TestApiKey);

            // Act
            JObject json;
            using (var server = new TestServer(client))
            {
                client.Notify(StaticData.TestInnerException);
                json = server.GetLastResponse();
            }

            // Assert
            Assert.Equal(StaticData.TestApiKey, json["apiKey"]);
            Assert.Equal(3, json["events"][0]["exceptions"].Count());
            Assert.Equal("ArithmeticException", json["events"][0]["exceptions"][0]["errorClass"]);
            Assert.Equal("DivideByZeroException", json["events"][0]["exceptions"][1]["errorClass"]);
            Assert.Equal("TypeAccessException", json["events"][0]["exceptions"][2]["errorClass"]);
        }

        [Fact]
        public void CheckInnerExceptionsWithNoStackTraceAreNotified()
        {
            // Arrange
            var client = new BaseClient(StaticData.TestApiKey);

            // Act
            JObject json;
            using (var server = new TestServer(client))
            {
                client.Notify(StaticData.TestInnerNoStackException);
                json = server.GetLastResponse();
            }

            // Assert
            Assert.Equal(StaticData.TestApiKey, json["apiKey"]);
            Assert.Equal(3, json["events"][0]["exceptions"].Count());
            Assert.Equal("DivideByZeroException", json["events"][0]["exceptions"][0]["errorClass"].ToString());
            Assert.Equal("TimeoutException", json["events"][0]["exceptions"][1]["errorClass"].ToString());
            Assert.Equal("OperationCanceledException", json["events"][0]["exceptions"][2]["errorClass"].ToString());
        }

        [Fact]
        public void CheckInnerExceptionsForBasicAggregateException()
        {
            // Arrange
            var client = new BaseClient(StaticData.TestApiKey);
            AggregateException testAggregateException = null;
            var task1 = Task.Factory.StartNew(() => { throw new FieldAccessException("Task 1 Exception"); });
            var task2 = Task.Factory.StartNew(() => { throw new StackOverflowException("Task 2 Exception"); });
            var task3 = Task.Factory.StartNew(() => { throw new AccessViolationException("Task 3 Exception"); });
            try
            {
                Task.WaitAll(task1, task2, task3);
            }
            catch (AggregateException ae)
            {
                testAggregateException = ae;
            }

            // Act
            JObject json;
            using (var server = new TestServer(client))
            {
                client.Notify(testAggregateException);
                json = server.GetLastResponse();
            }

            // Assert
            Assert.Equal(StaticData.TestApiKey, json["apiKey"]);
            Assert.Equal(1, json["events"].Count());

            // Check that 3 events are created, each having the Aggregate exception as the root exception
            var task1Exps = json["events"][0]["exceptions"].First(x => x["errorClass"].Value<string>() == "FieldAccessException");
            var task2Exps = json["events"][0]["exceptions"].First(x => x["errorClass"].Value<string>() == "StackOverflowException");
            var task3Exps = json["events"][0]["exceptions"].First(x => x["errorClass"].Value<string>() == "AccessViolationException");

            Assert.Equal("AggregateException", json["events"][0]["exceptions"][0]["errorClass"]);

            Assert.Equal("Task 1 Exception", task1Exps["message"]);
            Assert.Equal("Task 2 Exception", task2Exps["message"]);
            Assert.Equal("Task 3 Exception", task3Exps["message"]);
        }

#if DEBUG
        [Fact]
        public void CheckCallStacksAreReportedCorrectly()
        {
            // Arrange
            var client = new BaseClient(StaticData.TestApiKey);

            // Act
            JObject json;
            using (var server = new TestServer(client))
            {
                client.Notify(StaticData.TestCallStackException);
                json = server.GetLastResponse();
            }

            // Assert
            Assert.Equal(StaticData.TestApiKey, json["apiKey"]);

            var traceJson = json["events"][0]["exceptions"][0]["stacktrace"];

            Assert.Equal(3, traceJson.Count());
            Assert.Equal("TestNamespace.ClassGamma.ThrowException()", traceJson[0]["method"]);
            Assert.Equal("TestNamespace.ClassBeta.ThrowException()", traceJson[1]["method"]);
            Assert.Equal("TestNamespace.ClassAlpha.ThrowException()", traceJson[2]["method"]);
        }

#endif
    }
}
