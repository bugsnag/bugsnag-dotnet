using Xunit;

namespace Bugsnag.Core.Test
{
    public class MetaDataTests
    {
        [Fact]
        public void Constructor_BlankMetadataContainsEmptyDataStore()
        {
            // Arrange
            var testMetaData = new MetaData();

            // Assert
            Assert.NotNull(testMetaData.MetaDataStore);
            Assert.Equal(0, testMetaData.MetaDataStore.Count);
        }
    }
}
