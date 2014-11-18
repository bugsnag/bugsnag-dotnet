using Xunit;

namespace Bugsnag.Core.Test
{
    public class MetaDataTests
    {
        [Fact]
        public void Constructor_NewMetadataContainsEmptyDataStore()
        {
            // Arrange + Act
            var testMetadata = new Metadata();

            // Assert
            Assert.NotNull(testMetadata.MetadataStore);
            Assert.Equal(0, testMetadata.MetadataStore.Count);
        }

        [Fact]
        public void AddToTab_AddingTabEntryToNewMetadata()
        {
            // Arrange
            var testMetadata = new Metadata();
            var testTabObject = new Object();

            // Act
            testMetadata.AddToTab("Test Tab", "Test Key", testTabObject);

            // Assert
            var testStore = testMetadata.MetadataStore;
            Assert.Equal(1, testStore.Count);
            Assert.Equal(1, testStore["Test Tab"].Count);
            Assert.Equal(testTabObject, testStore["Test Tab"]["Test Key"]);
        }
        
        [Fact]
        public void AddToTab_AddingMultipleDifferentTabEntrysToSingleTab()
        {
            // Arrange
            var testMetadata = new Metadata();
            var testTabObject1 = new Object();
            var testTabObject2 = new Object();
            var testTabObject3 = new Object();

            // Act
            testMetadata.AddToTab("Test Tab", "Test Key 1", testTabObject1);
            testMetadata.AddToTab("Test Tab", "Test Key 2", testTabObject2);
            testMetadata.AddToTab("Test Tab", "Test Key 3", testTabObject3);

            // Assert
            var testStore = testMetadata.MetadataStore;
            Assert.Equal(1, testStore.Count);
            Assert.Equal(3, testStore["Test Tab"].Count);
            Assert.Equal(testTabObject1, testStore["Test Tab"]["Test Key 1"]);
            Assert.Equal(testTabObject2, testStore["Test Tab"]["Test Key 2"]);
            Assert.Equal(testTabObject3, testStore["Test Tab"]["Test Key 3"]);
        }
        
        [Fact]
        public void AddToTab_AddingMultipleDifferentTabsEntryToMultipleTabs()
        {
            // Arrange
            var testMetadata = new Metadata();
            var testTab1Object1 = new Object();
            var testTab1Object2 = new Object();
            var testTab2Object1 = new Object();
            var testTab3Object1 = new Object();
            var testTab3Object2 = new Object();
            var testTab3Object3 = new Object();

            // Act
            testMetadata.AddToTab("Test Tab 1", "Test Key 1", testTab1Object1);
            testMetadata.AddToTab("Test Tab 1", "Test Key 2", testTab1Object2);
            testMetadata.AddToTab("Test Tab 2", "Test Key 1", testTab2Object1);
            testMetadata.AddToTab("Test Tab 3", "Test Key 1", testTab3Object1);
            testMetadata.AddToTab("Test Tab 3", "Test Key 2", testTab3Object2);
            testMetadata.AddToTab("Test Tab 3", "Test Key 3", testTab3Object3);

            // Assert
            var testStore = testMetadata.MetadataStore;
            Assert.Equal(3, testStore.Count);
            Assert.Equal(2, testStore["Test Tab 1"].Count);
            Assert.Equal(1, testStore["Test Tab 2"].Count);
            Assert.Equal(3, testStore["Test Tab 3"].Count);
            Assert.Equal(testTab1Object1, testStore["Test Tab 1"]["Test Key 1"]);
            Assert.Equal(testTab1Object2, testStore["Test Tab 1"]["Test Key 2"]);
            Assert.Equal(testTab2Object1, testStore["Test Tab 2"]["Test Key 1"]);
            Assert.Equal(testTab3Object1, testStore["Test Tab 3"]["Test Key 1"]);
            Assert.Equal(testTab3Object2, testStore["Test Tab 3"]["Test Key 2"]);
            Assert.Equal(testTab3Object3, testStore["Test Tab 3"]["Test Key 3"]);
        }
        
        [Fact]
        public void AddToTab_AddingEntryWithSameKeyOverwritesPreviousEntry()
        {
            // Arrange
            var testMetadata = new Metadata();
            var testTabObject1 = new Object();
            var testTabObject2 = new Object();
            var testTabObject3 = new Object();

            // Act
            testMetadata.AddToTab("Test Tab", "Test Key 1", testTabObject1);
            testMetadata.AddToTab("Test Tab", "Test Key 1", testTabObject2);
            testMetadata.AddToTab("Test Tab", "Test Key 1", testTabObject3);

            // Assert
            var testStore = testMetadata.MetadataStore;
            Assert.Equal(1, testStore.Count);
            Assert.Equal(1, testStore["Test Tab"].Count);
            Assert.Equal(testTabObject3, testStore["Test Tab"]["Test Key 1"]);
        }
    }
}
