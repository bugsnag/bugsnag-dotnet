using System;
using System.Collections.Generic;
using Bugsnag.Clients;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Core.Test.FunctionalTests
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
                server.ListenForResponse();
                client.Notify(exp);
                json = server.Response();
            }

            // Assert
            Assert.Equal(TestApiKey, json["apiKey"]);
            Assert.Equal(Notifier.Name, json["notifier"]["name"]);
            Assert.Equal(Notifier.Version, json["notifier"]["version"]);
            Assert.Equal(Notifier.Url.AbsoluteUri, json["notifier"]["url"]);
        }
    }
}
