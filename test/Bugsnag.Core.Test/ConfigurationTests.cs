using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Core.Test
{
    public class ConfigurationTests
    {
        [Theory]
        [InlineData("Random API Key")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        public void Constructor_CheckApiKeyIsRecordedCorrectly(string apiKey)
        {
            // Arrange + Act
            var testConfig = new Configuration(apiKey);

            // Assert
            Assert.Equal(apiKey, testConfig.ApiKey);
        }

        [Theory]
        [InlineData("stage 1")]
        [InlineData("stage 2")]
        [InlineData("stage 4")]
        [InlineData("stage 7")]
        [InlineData(null)]
        public void ReleaseStage_IdentifiesReleaseStagesToNotifyOn(string stage)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.SetNotifyReleaseStages("stage 1", "stage 2", "stage 4", "stage 7");
            testConfig.ReleaseStage = stage;

            // Assert
            Assert.True(testConfig.IsNotifyReleaseStage());
        }

        [Theory]
        [InlineData("stage 3")]
        [InlineData("stage 5")]
        [InlineData("stage 6")]
        [InlineData("stage 8")]
        [InlineData("")]
        public void ReleaseStage_IdentifiesReleaseStagesToNotNotifyOn(string stage)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.SetNotifyReleaseStages("stage 1", "stage 2", "stage 4", "stage 7");
            testConfig.ReleaseStage = stage;

            // Assert
            Assert.False(testConfig.IsNotifyReleaseStage());
        }

        [Theory]
        [InlineData("stage 4")]
        [InlineData("stage 5")]
        [InlineData("stage 6")]
        [InlineData("")]
        public void ReleaseStage_ClearingReleaseStagesCauseAllStagesToNotify(string stage)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.SetNotifyReleaseStages("stage 1", "stage 2", "stage 3");
            testConfig.ReleaseStage = stage;
            Assert.False(testConfig.IsNotifyReleaseStage());
            testConfig.NotifyOnAllReleaseStages();

            // Assert
            Assert.True(testConfig.IsNotifyReleaseStage());
        }

        [Theory]
        [InlineData("stage 1")]
        [InlineData("stage 2")]
        [InlineData("stage 3")]
        [InlineData("")]
        public void ReleaseStage_AllReleaseStageNotifyByDefault(string stage)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.ReleaseStage = stage;

            // Assert
            Assert.True(testConfig.IsNotifyReleaseStage());
        }

        [Theory]
        [InlineData("another.point.com", true, "https://another.point.com/")]
        [InlineData("another.point.com", false, "http://another.point.com/")]
        public void FinalUrl_UrlIsCreatedCorrectly(string endpoint, bool useSsl, string url)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.Endpoint = endpoint;
            testConfig.UseSsl = useSsl;

            // Assert
            Assert.Equal(url, testConfig.FinalUrl.AbsoluteUri);
        }
    }
}
