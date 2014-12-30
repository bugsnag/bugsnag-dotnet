using System;
using System.Collections.Generic;
using Bugsnag.Clients;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Test.FunctionalTests
{
    public class BasicClientTests
    {
        public const string TestApiKey = "ABCDEF1234567890ABCDEF1234567890";

        public static readonly RankException TestException1;
        public static readonly SystemException TestException2;

        static BasicClientTests()
        {
            // Initialise test exceptions
            try
            {
                throw new RankException("Rank Test");
            }
            catch (RankException e)
            {
                TestException1 = e;
            }

            TestException2 = new SystemException("System Test Exception");
        }

        public static IEnumerable<object[]> ExceptionData
        {
            get
            {
                return new[]
                {
                    new object[] {TestException1},
                    new object[] {TestException2}
                };
            }
        }

        [Theory]
        [PropertyData("ExceptionData")]
        public void CheckBasicPropertiesOfNotifications(Exception exp)
        {
            // Arrange
            var client = new BaseClient(TestApiKey);

            // Act
            JObject json;
            using (var server = new TestServer(client))
            {
                client.Notify(exp);
                json = server.GetLastResponse();
            }

            // Assert
            Assert.Equal(TestApiKey, json["apiKey"]);
            Assert.Equal(Notifier.Name, json["notifier"]["name"]);
            Assert.Equal(Notifier.Version, json["notifier"]["version"]);
            Assert.Equal(Notifier.Url.AbsoluteUri, json["notifier"]["url"]);
        }

        [Fact]
        public void DefaultSeverityForManuallyNotifiedExceptionsIsWarning()
        {
            // Arrange
            var client = new BaseClient(TestApiKey);

            // Act
            JObject json;
            using (var server = new TestServer(client))
            {
                client.Notify(TestException1);
                json = server.GetLastResponse();
            }

            // Assert
            Assert.Equal("warning", json["events"][0]["severity"]);
        }

        [Theory]
        [InlineData(Severity.Info, "info")]
        [InlineData(Severity.Error, "error")]
        [InlineData(Severity.Warning, "warning")]
        public void SeveritySetManuallyIsUsedInTheNotification(Severity severity, string expJsonString)
        {
            // Arrange
            var client = new BaseClient(TestApiKey);

            // Act
            JObject json;
            using (var server = new TestServer(client))
            {
                client.Notify(TestException1, severity);
                json = server.GetLastResponse();
            }

            // Assert
            Assert.Equal(expJsonString, json["events"][0]["severity"]);
        }

        [Fact]
        public void AddSimpleMetadataToANotifications()
        {
            // Arrange
            var client = new BaseClient(TestApiKey);
            var testMetadata = new Metadata();
            testMetadata.AddToTab("Test Tab 1", "Key 1", "Value 1");
            testMetadata.AddToTab("Test Tab 1", "Key 2", "Value 2");
            testMetadata.AddToTab("Test Tab 2", "Key 1", "Value 1");

            // Act
            JObject json;
            using (var server = new TestServer(client))
            {
                client.Notify(TestException1, testMetadata);
                json = server.GetLastResponse();
            }

            // Assert
            var actData = json["events"][0]["metaData"];
            Assert.Equal("Value 1", actData["Test Tab 1"]["Key 1"]);
            Assert.Equal("Value 2", actData["Test Tab 1"]["Key 2"]);
            Assert.Equal("Value 1", actData["Test Tab 2"]["Key 1"]);
        }
    }
}
