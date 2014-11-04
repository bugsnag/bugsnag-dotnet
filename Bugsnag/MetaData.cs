using System;
using System.Collections.Generic;
using System.Linq;
using InternalMetaData = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, object>>;

namespace Bugsnag
{
    public class MetaData
    {
        private const string DefaultTabName = "Custom Data";

        public InternalMetaData MetaDataStore {get; private set;}

        public MetaData()
        {
            MetaDataStore = new InternalMetaData();
        }

        public void AddToTab(string tabEntryKey, object tabEntryValue)
        {
            AddToTab(DefaultTabName, tabEntryKey, tabEntryValue);
        }

        public void AddToTab(string tabName, string tabEntryKey, object tabEntryValue)
        {
            if (MetaDataStore.ContainsKey(tabName))
            {
                if (MetaDataStore[tabName].ContainsKey(tabEntryKey))
                    throw new ArgumentException("Unable to add to tab, tab entry already exists");

                MetaDataStore[tabName].Add(tabEntryKey, tabEntryValue);
            }
            else
            {
                var newTabData = new Dictionary<string, object> { { tabEntryKey, tabEntryValue } };
                MetaDataStore.Add(tabName, newTabData);
            }
        }

        public void RemoveTab(string tabName)
        {
            if (!MetaDataStore.ContainsKey(tabName))
                throw new ArgumentException("Unable to remove tab, tab does not exist");

            MetaDataStore.Remove(tabName);
        }

        public void RemoveTabEntry(string tabName, string tabEntryKey)
        {
            if (!MetaDataStore.ContainsKey(tabName))
                throw new ArgumentException("Unable to remove tab entry, tab does not exist");

            if (!MetaDataStore[tabName].ContainsKey(tabEntryKey))
                throw new ArgumentException("Unable to remove tab entry , tab does not exist");

            MetaDataStore[tabName].Remove(tabEntryKey);
        }

        public static MetaData MergeMetaData(params MetaData[] data)
        {
            var aggData = data.ToList();
            aggData.Insert(0, new MetaData());
            return aggData.Aggregate(Merge);
        }

        private static MetaData Merge(MetaData currentData, MetaData dataToAdd)
        {
            var currStore = currentData.MetaDataStore;
            var storeToAdd = dataToAdd.MetaDataStore;

            // Loop through all the tabs that are in the data to add...
            foreach (var newTab in storeToAdd)
            {
                // If the tab doesn't exist in current data, add a blank tab
                if (!currStore.ContainsKey(newTab.Key))
                    currStore.Add(newTab.Key, new Dictionary<string, object>());

                var currTab = currStore[newTab.Key];

                // Loop through all the entries in the tab to add...
                foreach(var newTabEntry in newTab.Value)
                {
                    // If the current data has the same key, fail the merge, otherwise add it
                    if (currTab.ContainsKey(newTabEntry.Key))
                        throw new InvalidOperationException(
                            String.Format("Could not merge data for tab {0}, duplicate data for tab entry {1}", newTabEntry.Key, newTabEntry));

                    currTab.Add(newTabEntry.Key, newTabEntry.Value);
                }
            }
            return currentData;
        }
    }
}
