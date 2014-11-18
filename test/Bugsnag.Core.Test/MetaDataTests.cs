using System;
using Xunit;
using Xunit.Extensions;

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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void AddToTab_AddingTabEntryToNewMetadata(bool useDefaultTab)
        {
            // Arrange
            var testMetadata = new Metadata();
            var testTabObject = new Object();
            var tabName = useDefaultTab ? Metadata.DefaultTabName : "Test Tab";

            // Act
            if (useDefaultTab)
                testMetadata.AddToTab("Test Key", testTabObject);
            else
                testMetadata.AddToTab("Test Tab", "Test Key", testTabObject);

            // Assert
            var testStore = testMetadata.MetadataStore;
            Assert.Equal(1, testStore.Count);
            Assert.Equal(1, testStore[tabName].Count);
            Assert.Equal(testTabObject, testStore[tabName]["Test Key"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void AddToTab_AddingMultipleDifferentTabEntrysToSingleTab(bool useDefaultTab)
        {
            // Arrange
            var testMetadata = new Metadata();
            var testTabObject1 = "Random String";
            var testTabObject2 = 34;
            var testTabObject3 = new Object[]{23, 24, 25, "Rt"};
            var tabName = useDefaultTab ? Metadata.DefaultTabName : "Test Tab";

            // Act
            if (useDefaultTab)
            {
                testMetadata.AddToTab("Test Key 1", testTabObject1);
                testMetadata.AddToTab("Test Key 2", testTabObject2);
                testMetadata.AddToTab("Test Key 3", testTabObject3);
            }
            else
            {
                testMetadata.AddToTab("Test Tab", "Test Key 1", testTabObject1);
                testMetadata.AddToTab("Test Tab", "Test Key 2", testTabObject2);
                testMetadata.AddToTab("Test Tab", "Test Key 3", testTabObject3);
            }

            // Assert
            var testStore = testMetadata.MetadataStore;
            Assert.Equal(1, testStore.Count);
            Assert.Equal(3, testStore[tabName].Count);
            Assert.Equal(testTabObject1, testStore[tabName]["Test Key 1"]);
            Assert.Equal(testTabObject2, testStore[tabName]["Test Key 2"]);
            Assert.Equal(testTabObject3, testStore[tabName]["Test Key 3"]);
        }
        
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void AddToTab_AddingMultipleDifferentTabsEntryToMultipleTabs(bool useDefaultTab)
        {
            // Arrange
            var testMetadata = new Metadata();
            var testTab1Object1 = new Object();
            var testTab1Object2 = 5;
            var testTab2Object1 = new [] {"a", "b", "c"};
            var testTab3Object1 = "My test";
            var testTab3Object2 = DateTime.Now;
            var testTab3Object3 = new Object();

            var firstTabName = useDefaultTab ? Metadata.DefaultTabName : "Test Tab 1";

            // Act
            if (useDefaultTab)
            {
                 testMetadata.AddToTab("Test Key 1", testTab1Object1);
                 testMetadata.AddToTab("Test Key 2", testTab1Object2);
            }
            else
            {
                 testMetadata.AddToTab("Test Tab 1", "Test Key 1", testTab1Object1);
                 testMetadata.AddToTab("Test Tab 1", "Test Key 2", testTab1Object2);
            }
            testMetadata.AddToTab("Test Tab 2", "Test Key 1", testTab2Object1);
            testMetadata.AddToTab("Test Tab 3", "Test Key 1", testTab3Object1);
            testMetadata.AddToTab("Test Tab 3", "Test Key 2", testTab3Object2);
            testMetadata.AddToTab("Test Tab 3", "Test Key 3", testTab3Object3);

            // Assert
            var testStore = testMetadata.MetadataStore;
            Assert.Equal(3, testStore.Count);
            Assert.Equal(2, testStore[firstTabName].Count);
            Assert.Equal(1, testStore["Test Tab 2"].Count);
            Assert.Equal(3, testStore["Test Tab 3"].Count);
            Assert.Equal(testTab1Object1, testStore[firstTabName]["Test Key 1"]);
            Assert.Equal(testTab1Object2, testStore[firstTabName]["Test Key 2"]);
            Assert.Equal(testTab2Object1, testStore["Test Tab 2"]["Test Key 1"]);
            Assert.Equal(testTab3Object1, testStore["Test Tab 3"]["Test Key 1"]);
            Assert.Equal(testTab3Object2, testStore["Test Tab 3"]["Test Key 2"]);
            Assert.Equal(testTab3Object3, testStore["Test Tab 3"]["Test Key 3"]);
        }
        
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void AddToTab_AddingEntryWithSameKeyOverwritesPreviousEntry(bool useDefaultTab)
        {
            // Arrange
            var testMetadata = new Metadata();
            var testTabObject1 = 999999;
            var testTabObject2 = "AAAAAAAA";
            var testTabObject3 = new Object[] { testTabObject2 };
            var tabName = useDefaultTab ? Metadata.DefaultTabName : "Test Tab";

            // Act
            if (useDefaultTab)
            {
                testMetadata.AddToTab("Test Key 1", testTabObject1);
                testMetadata.AddToTab("Test Key 1", testTabObject2);
                testMetadata.AddToTab("Test Key 1", testTabObject3);
            }
            else
            {
                testMetadata.AddToTab("Test Tab", "Test Key 1", testTabObject1);
                testMetadata.AddToTab("Test Tab", "Test Key 1", testTabObject2);
                testMetadata.AddToTab("Test Tab", "Test Key 1", testTabObject3);
            }

            // Assert
            var testStore = testMetadata.MetadataStore;
            Assert.Equal(1, testStore.Count);
            Assert.Equal(1, testStore[tabName].Count);
            Assert.Equal(testTabObject3, testStore[tabName]["Test Key 1"]);
        }
    }
}
