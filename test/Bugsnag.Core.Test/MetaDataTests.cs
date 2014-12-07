using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Core.Test
{
    public class MetadataTests
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
            var testTabObject3 = new Object[] { 23, 24, 25, "Rt" };
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
            var testTab2Object1 = new[] { "a", "b", "c" };
            var testTab3Object1 = "My test";
            var testTab3Object2 = DateTime.Now;
            var testTab3Object3 = new Object();

            // If we using the default tab, only use it for the first tab
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

        [Theory]
        [InlineData("Tab 4")]
        [InlineData("Tab 5")]
        [InlineData("Random Tab")]
        [InlineData("")]
        [InlineData(null)]
        public void RemoveTab_RemoveTabWhenTabDoesntExistDoesNothing(string tabToRemove)
        {
            // Arrange
            var testBlankMetadata = new Metadata();
            var testFullMetadata = new Metadata();
            testFullMetadata.MetadataStore.Add("Tab 1", new Dictionary<string, object>() { { "T1", "V1" } });
            testFullMetadata.MetadataStore.Add("Tab 2", new Dictionary<string, object>() { { "T2", "V2" } });
            testFullMetadata.MetadataStore.Add("Tab 3", new Dictionary<string, object>() { { "T3", "V3" } });

            // Act
            testBlankMetadata.RemoveTab(tabToRemove);
            testFullMetadata.RemoveTab(tabToRemove);

            // Assert
            Assert.Equal(0, testBlankMetadata.MetadataStore.Count);

            Assert.Equal(3, testFullMetadata.MetadataStore.Count);
            Assert.Equal(1, testFullMetadata.MetadataStore["Tab 1"].Count);
            Assert.Equal(1, testFullMetadata.MetadataStore["Tab 2"].Count);
            Assert.Equal(1, testFullMetadata.MetadataStore["Tab 3"].Count);

            Assert.Equal("V1", testFullMetadata.MetadataStore["Tab 1"]["T1"]);
            Assert.Equal("V2", testFullMetadata.MetadataStore["Tab 2"]["T2"]);
            Assert.Equal("V3", testFullMetadata.MetadataStore["Tab 3"]["T3"]);
        }

        [Fact]
        public void RemoveTab_RemoveTabSuccessfullyFromMetadata()
        {
            // Arrange
            var testMetadata = new Metadata();
            var obj1_1 = new Object();
            var obj2_1 = new Object();
            var obj2_2 = new Object();
            var obj3_1 = new Object();
            var obj3_2 = new Object();
            var obj3_3 = new Object();
            testMetadata.MetadataStore.Add("Tab 1", new Dictionary<string, object>() { { "T1_1", obj1_1 } });
            testMetadata.MetadataStore.Add("Tab 2", new Dictionary<string, object>() { { "T2_1", obj2_1 }, { "T2_2", obj2_2 } });
            testMetadata.MetadataStore.Add("Tab 3", new Dictionary<string, object>() { { "T3_1", obj3_1 }, { "T3_2", obj3_2 }, { "T3_3", obj3_3 } });

            // Act
            testMetadata.RemoveTab("Tab 1");

            // Assert
            Assert.Equal(2, testMetadata.MetadataStore.Count);

            Assert.Equal(2, testMetadata.MetadataStore["Tab 2"].Count);
            Assert.Equal(3, testMetadata.MetadataStore["Tab 3"].Count);

            Assert.Equal(obj2_1, testMetadata.MetadataStore["Tab 2"]["T2_1"]);
            Assert.Equal(obj2_2, testMetadata.MetadataStore["Tab 2"]["T2_2"]);
            Assert.Equal(obj3_1, testMetadata.MetadataStore["Tab 3"]["T3_1"]);
            Assert.Equal(obj3_2, testMetadata.MetadataStore["Tab 3"]["T3_2"]);
            Assert.Equal(obj3_3, testMetadata.MetadataStore["Tab 3"]["T3_3"]);

            // Act
            testMetadata.RemoveTab("Tab 2");

            // Assert
            Assert.Equal(1, testMetadata.MetadataStore.Count);
            Assert.Equal(3, testMetadata.MetadataStore["Tab 3"].Count);
            Assert.Equal(obj3_1, testMetadata.MetadataStore["Tab 3"]["T3_1"]);
            Assert.Equal(obj3_2, testMetadata.MetadataStore["Tab 3"]["T3_2"]);
            Assert.Equal(obj3_3, testMetadata.MetadataStore["Tab 3"]["T3_3"]);

            // Act
            testMetadata.RemoveTab("Tab 3");

            // Assert
            Assert.Equal(0, testMetadata.MetadataStore.Count);
        }

        [Theory]
        [InlineData("Tab 4", "Entry 1")]
        [InlineData("Tab 5", "T1")]
        [InlineData("Random Tab", null)]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("Tab 1", "Random")]
        [InlineData("Tab 2", "T3")]
        [InlineData("Tab 3", null)]
        [InlineData("Tab 2", "")]
        public void RemoveTabEntry_RemoveTabEntryWhenTabOrEntryDoesntExistDoesNothing(string tab, string entryToRemove)
        {
            // Arrange
            var testBlankMetadata = new Metadata();
            var testFullMetadata = new Metadata();
            testFullMetadata.MetadataStore.Add("Tab 1", new Dictionary<string, object>() { { "T1", "V1" } });
            testFullMetadata.MetadataStore.Add("Tab 2", new Dictionary<string, object>() { { "T2", "V2" } });
            testFullMetadata.MetadataStore.Add("Tab 3", new Dictionary<string, object>() { { "T3", "V3" } });

            // Act
            testBlankMetadata.RemoveTabEntry(tab, entryToRemove);
            testFullMetadata.RemoveTabEntry(tab, entryToRemove);

            // Assert
            Assert.Equal(0, testBlankMetadata.MetadataStore.Count);

            Assert.Equal(3, testFullMetadata.MetadataStore.Count);
            Assert.Equal(1, testFullMetadata.MetadataStore["Tab 1"].Count);
            Assert.Equal(1, testFullMetadata.MetadataStore["Tab 2"].Count);
            Assert.Equal(1, testFullMetadata.MetadataStore["Tab 3"].Count);

            Assert.Equal("V1", testFullMetadata.MetadataStore["Tab 1"]["T1"]);
            Assert.Equal("V2", testFullMetadata.MetadataStore["Tab 2"]["T2"]);
            Assert.Equal("V3", testFullMetadata.MetadataStore["Tab 3"]["T3"]);
        }

        [Fact]
        public void RemoveTabEntry_RemoveTabEntrySuccessfullyFromMetadata()
        {
            // Arrange
            var testMetadata = new Metadata();
            var obj1_1 = new Object();
            var obj2_1 = new Object();
            var obj2_2 = new Object();
            var obj3_1 = new Object();
            var obj3_2 = new Object();
            var obj3_3 = new Object();
            testMetadata.MetadataStore.Add("Tab 1", new Dictionary<string, object>() { { "T1_1", obj1_1 } });
            testMetadata.MetadataStore.Add("Tab 2", new Dictionary<string, object>() { { "T2_1", obj2_1 }, { "T2_2", obj2_2 } });
            testMetadata.MetadataStore.Add("Tab 3", new Dictionary<string, object>() { { "T3_1", obj3_1 }, { "T3_2", obj3_2 }, { "T3_3", obj3_3 } });

            // Act
            testMetadata.RemoveTabEntry("Tab 2", "T2_1");
            testMetadata.RemoveTabEntry("Tab 3", "T3_2");

            // Assert
            Assert.Equal(3, testMetadata.MetadataStore.Count);
            Assert.Equal(1, testMetadata.MetadataStore["Tab 1"].Count);
            Assert.Equal(1, testMetadata.MetadataStore["Tab 2"].Count);
            Assert.Equal(2, testMetadata.MetadataStore["Tab 3"].Count);

            Assert.Equal(obj1_1, testMetadata.MetadataStore["Tab 1"]["T1_1"]);
            Assert.Equal(obj2_2, testMetadata.MetadataStore["Tab 2"]["T2_2"]);
            Assert.Equal(obj3_1, testMetadata.MetadataStore["Tab 3"]["T3_1"]);
            Assert.Equal(obj3_3, testMetadata.MetadataStore["Tab 3"]["T3_3"]);
        }

        [Fact]
        public void RemoveTabEntry_RemovingLastEntryWillRemoveTab()
        {
            // Arrange
            var testMetadata = new Metadata();
            var obj1 = new Object();
            var obj2 = new Object();
            testMetadata.MetadataStore.Add("Tab 1", new Dictionary<string, object>() { { "T1", obj1 }, { "T2", obj2 } });

            // Act
            testMetadata.RemoveTabEntry("Tab 1", "T2");

            // Assert
            Assert.Equal(1, testMetadata.MetadataStore.Count);
            Assert.Equal(1, testMetadata.MetadataStore["Tab 1"].Count);
            Assert.Equal(obj1, testMetadata.MetadataStore["Tab 1"]["T1"]);

            // Act
            testMetadata.RemoveTabEntry("Tab 1", "T1");

            // Assert
            Assert.Equal(0, testMetadata.MetadataStore.Count);
        }

        [Fact]
        public void FilterEntries_NullPredicateDoesNothingToMetadata()
        {
            // Arrange
            var testMetadata = new Metadata();
            var obj1 = new Object();
            var obj2 = new Object();
            testMetadata.MetadataStore.Add("Tab 1", new Dictionary<string, object>() { { "T1", obj1 }, { "T2", obj2 } });

            // Act
            testMetadata.FilterEntries(null);

            // Assert
            Assert.Equal(1, testMetadata.MetadataStore.Count);
            Assert.Equal(2, testMetadata.MetadataStore["Tab 1"].Count);
            Assert.Equal(obj1, testMetadata.MetadataStore["Tab 1"]["T1"]);
            Assert.Equal(obj2, testMetadata.MetadataStore["Tab 1"]["T2"]);
        }

        [Fact]
        public void FilterEntries_FiltersAllEntriesThatEndWithASpecificValue()
        {
            // Arrange
            var testMetadata = new Metadata();
            var obj1_1 = new Object();
            var obj2_1 = new Object();
            var obj2_2 = new Object();
            var obj3_1 = new Object();
            var obj3_2 = new Object();
            var obj3_3 = new Object();
            testMetadata.MetadataStore.Add("Tab 1", new Dictionary<string, object>() { { "T1_1", obj1_1 } });
            testMetadata.MetadataStore.Add("Tab 2", new Dictionary<string, object>() { { "T2_1", obj2_1 }, { "T2_2", obj2_2 } });
            testMetadata.MetadataStore.Add("Tab 3", new Dictionary<string, object>() { { "T3_1", obj3_1 }, { "T3_2", obj3_2 }, { "T3_3", obj3_3 } });
            Func<string, bool> filter = (key) => key.EndsWith("2");

            // Act
            testMetadata.FilterEntries(filter);

            // Assert
            Assert.Equal(3, testMetadata.MetadataStore.Count);
            Assert.Equal(1, testMetadata.MetadataStore["Tab 1"].Count);
            Assert.Equal(2, testMetadata.MetadataStore["Tab 2"].Count);
            Assert.Equal(3, testMetadata.MetadataStore["Tab 3"].Count);

            Assert.Equal(obj1_1, testMetadata.MetadataStore["Tab 1"]["T1_1"]);
            Assert.Equal(obj2_1, testMetadata.MetadataStore["Tab 2"]["T2_1"]);
            Assert.Equal("[FILTERED]", testMetadata.MetadataStore["Tab 2"]["T2_2"]);
            Assert.Equal(obj3_1, testMetadata.MetadataStore["Tab 3"]["T3_1"]);
            Assert.Equal("[FILTERED]", testMetadata.MetadataStore["Tab 3"]["T3_2"]);
            Assert.Equal(obj3_3, testMetadata.MetadataStore["Tab 3"]["T3_3"]);
        }

        [Fact]
        public void FilterEntries_RecordExceptionInEntryIfPredicateThrowsException()
        {
            // Arrange
            var testMetadata = new Metadata();
            var obj1 = new Object();
            var obj2 = new Object();
            var obj3 = new Object();
            testMetadata.MetadataStore.Add("Tab 1", new Dictionary<string, object>() { { "T1", obj1 }, { "T2", obj2 }, { "T3", obj3 } });

            Func<string, bool> filter = (key) =>
            {
                if (key == "T1")
                    return false;
                if (key == "T2")
                    throw new SystemException("System Test Error");
                return true;
            };

            // Act
            testMetadata.FilterEntries(filter);

            // Assert
            Assert.Equal(1, testMetadata.MetadataStore.Count);
            Assert.Equal(3, testMetadata.MetadataStore["Tab 1"].Count);
            Assert.Equal(obj1, testMetadata.MetadataStore["Tab 1"]["T1"]);
            Assert.Equal("[FILTERED]", testMetadata.MetadataStore["Tab 1"]["T3"]);

            var expEntry = testMetadata.MetadataStore["Tab 1"]["T2"] as string;
            Assert.True(expEntry.StartsWith("[FILTER ERROR]"));
            Assert.True(expEntry.Contains("System Test Error"));
        }

        [Fact]
        public void CombineMetaData_CombiningNullAndEmptyMetadatasAlwaysReturnsAnEmptyMetadata()
        {
            // Act
            var actData1 = Metadata.CombineMetadata();
            var actData2 = Metadata.CombineMetadata(null);
            var actData3 = Metadata.CombineMetadata(new Metadata());
            var actData4 = Metadata.CombineMetadata(new Metadata(), null);
            var actData5 = Metadata.CombineMetadata(null, new Metadata());
            var actData6 = Metadata.CombineMetadata(null, null, null);
            var actData7 = Metadata.CombineMetadata(new Metadata(), new Metadata(), new Metadata());

            // Assert
            Assert.Equal(0, actData1.MetadataStore.Count);
            Assert.Equal(0, actData2.MetadataStore.Count);
            Assert.Equal(0, actData3.MetadataStore.Count);
            Assert.Equal(0, actData4.MetadataStore.Count);
            Assert.Equal(0, actData5.MetadataStore.Count);
            Assert.Equal(0, actData6.MetadataStore.Count);
            Assert.Equal(0, actData7.MetadataStore.Count);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CombineMetaData_CombineTwoMetadataObjectsSuccessfully(bool combine1with2)
        {
            // Arrange
            var testMd1 = new Metadata();
            var md1_obj1_1 = new Object();
            var md1_obj1_2 = new Object();
            var md1_shared_1 = new Object();
            var md1_shared_2 = new Object();
            testMd1.MetadataStore.Add("Tab 1", new Dictionary<string, object>() { { "DATA1_T1_1", md1_obj1_1 }, { "DATA1_T1_2", md1_obj1_2 } });
            testMd1.MetadataStore.Add("Shared", new Dictionary<string, object>() { { "S_1", md1_shared_1 }, { "S_2", md1_shared_2 } });

            var testMd2 = new Metadata();
            var md2_obj2_1 = new Object();
            var md2_obj2_2 = new Object();
            var md2_shared_2 = new Object();
            var md2_shared_3 = new Object();
            testMd2.MetadataStore.Add("Tab 2", new Dictionary<string, object>() { { "DATA2_T2_1", md2_obj2_1 }, { "DATA2_T2_2", md2_obj2_2 } });
            testMd2.MetadataStore.Add("Shared", new Dictionary<string, object>() { { "S_2", md2_shared_2 }, { "S_3", md2_shared_3 } });

            // Act
            Metadata actData;
            if (combine1with2)
                actData = Metadata.CombineMetadata(testMd1, testMd2);
            else
                actData = Metadata.CombineMetadata(testMd2, testMd1);

            // Assert
            Assert.Equal(3, actData.MetadataStore.Count);

            Assert.Equal(2, actData.MetadataStore["Tab 1"].Count);
            Assert.Equal(2, actData.MetadataStore["Tab 2"].Count);
            Assert.Equal(3, actData.MetadataStore["Shared"].Count);

            Assert.Equal(md1_obj1_1, actData.MetadataStore["Tab 1"]["DATA1_T1_1"]);
            Assert.Equal(md1_obj1_2, actData.MetadataStore["Tab 1"]["DATA1_T1_2"]);
            Assert.Equal(md2_obj2_1, actData.MetadataStore["Tab 2"]["DATA2_T2_1"]);
            Assert.Equal(md2_obj2_2, actData.MetadataStore["Tab 2"]["DATA2_T2_2"]);

            var expShared2 = combine1with2 ? md2_shared_2 : md1_shared_2;

            Assert.Equal(md1_shared_1, actData.MetadataStore["Shared"]["S_1"]);
            Assert.Equal(expShared2, actData.MetadataStore["Shared"]["S_2"]);
            Assert.Equal(md2_shared_3, actData.MetadataStore["Shared"]["S_3"]);
        }
    }
}
