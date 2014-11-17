using Xunit;

namespace Bugsnag.Core.Test
{
    public class MetaDataTests
    {
        [Fact]
        public void Constructor_BlankMetadataContainsEmptyDataStore()
        {
            // Arrange + Act
            var testMetadata = new Metadata();

            // Assert
            Assert.NotNull(testMetadata.MetadataStore);
            Assert.Equal(0, testMetadata.MetadataStore.Count);
        }

        [Fact]
        public void AddToTab_AddingSingleTabEntryToBlankMetadataCreatesNewTab()
        {
            // Arrange
            var testMetadata = new Metadata();

            // Act
            testMetadata.AddToTab("Test Tab", "Test Key", "Test Value");

            // Assert
            var testStore = testMetadata.MetadataStore;
            Assert.Equal(1, testStore.Count);
            Assert.True(testStore.ContainsKey("Test Tab"));
            Assert.Equal(1, testStore["Test Tab"].Count);
            Assert.True(testStore["Test Tab"].ContainsKey("Test Key"));
            Assert.Equal("Test Value", testStore["Test Tab"]["Test Key"]);
        }
    }
}
