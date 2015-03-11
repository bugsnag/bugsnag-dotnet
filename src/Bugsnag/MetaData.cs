using System;
using System.Collections.Generic;
using System.Linq;
using TabData = System.Collections.Generic.Dictionary<string, object>;

namespace Bugsnag
{
    /// <summary>
    /// Used to store custom data that can be attached to an individual event
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// The tab name to use if a tab name is not supplied
        /// </summary>
        public const string DefaultTabName = "Custom Data";

        /// <summary>
        /// Gets the internal store used to represent the data. Can be modified directly and converted to JSON
        /// </summary>
        public Dictionary<string, TabData> MetadataStore { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Metadata"/> class. Default constructor
        /// </summary>
        public Metadata()
        {
            MetadataStore = new Dictionary<string, TabData>();
        }

        /// <summary>
        /// Merges metadata into a single metadata object. Later metadata objects in the list will have 
        /// precedence over earlier ones when merged data will identical keys. 
        /// Will not modify the metadata objects to be merged
        /// </summary>
        /// <param name="data">The metadata to merge</param>
        /// <returns>The merged metadata objects</returns>
        public static Metadata CombineMetadata(params Metadata[] data)
        {
            if (data == null)
                return new Metadata();

            // Create a list of metadata objects, with a blank one at the beginning.
            // This first blank metadata will have all other metadata merged into it
            var aggData = data.ToList();
            aggData.Insert(0, new Metadata());
            return aggData.Aggregate(Merge);
        }

        /// <summary>
        /// Adds an entry to the default tab
        /// </summary>
        /// <param name="tabEntryKey">The key of the entry</param>
        /// <param name="tabEntryValue">The object representing the entry</param>
        public void AddToTab(string tabEntryKey, object tabEntryValue)
        {
            AddToTab(DefaultTabName, tabEntryKey, tabEntryValue);
        }

        /// <summary>
        /// Adds an entry to a specific tab
        /// </summary>
        /// <param name="tabName">The tab to add the entry to</param>
        /// <param name="tabEntryKey">The key of the entry</param>
        /// <param name="tabEntryValue">The object representing the entry</param>
        public void AddToTab(string tabName, string tabEntryKey, object tabEntryValue)
        {
            // If the tab doesn't exist create a new tab with a single entry
            if (!MetadataStore.ContainsKey(tabName))
            {
                var newTabData = new Dictionary<string, object> { { tabEntryKey, tabEntryValue } };
                MetadataStore.Add(tabName, newTabData);
            }
            else
            {
                // If the tab entry exists, overwrite the entry otherwise add it as a new entry
                if (MetadataStore[tabName].ContainsKey(tabEntryKey))
                    MetadataStore[tabName][tabEntryKey] = tabEntryValue;
                else
                    MetadataStore[tabName].Add(tabEntryKey, tabEntryValue);
            }
        }

        /// <summary>
        /// Adds an existing metadata
        /// </summary>
        /// <param name="data">The metadata to add</param>
        public void AddMetadata(Metadata data)
        {
            Merge(this, data);
        }

        /// <summary>
        /// Removes a tab and all associated tab entries
        /// </summary>
        /// <param name="tabName">The name of the tab to remove</param>
        public void RemoveTab(string tabName)
        {
            if (tabName == null)
                return;

            // If the tab doesn't exist simply do nothing
            if (!MetadataStore.ContainsKey(tabName))
                return;

            MetadataStore.Remove(tabName);
        }

        /// <summary>
        /// Removes an individual tab entry from a tab. Removing the last entry, will remove the tab
        /// </summary>
        /// <param name="tabName">The name of the tab containing the entry to remove</param>
        /// <param name="tabEntryKey">The key of the tab entry to remove</param>
        public void RemoveTabEntry(string tabName, string tabEntryKey)
        {
            if (tabName == null || tabEntryKey == null)
                return;

            // If the tab doesn't exist or the tab entry doesn't exist simply do nothing
            if (!MetadataStore.ContainsKey(tabName) ||
                !MetadataStore[tabName].ContainsKey(tabEntryKey))
                return;

            MetadataStore[tabName].Remove(tabEntryKey);

            if (MetadataStore[tabName].Count == 0)
                RemoveTab(tabName);
        }

        /// <summary>
        /// Filter metadata by hiding selected tab entry values
        /// </summary>
        /// <param name="predicate">A predicate that, using the tab entry key,
        /// indicates an entry should be filtered.</param>
        public void FilterEntries(Func<string, bool> predicate)
        {
            // If there is no predicate, assume that no entries are to be filters
            if (predicate == null)
                return;

            // Loop through all tab entries in all tabs
            var tabs = MetadataStore.Keys.ToList();
            foreach (var tabName in tabs)
            {
                var entries = MetadataStore[tabName].Keys.ToList();
                foreach (var tabEntry in entries)
                {
                    var entryShouldBeFiltered = false;
                    try
                    {
                        entryShouldBeFiltered = predicate(tabEntry);
                    }
                    catch (Exception exp)
                    {
                        // If an error occurs while checking, we take no risks and replace the entry with
                        // the exception message
                        Console.WriteLine(String.Format("Exception while filtering entry {0} : {1},  {2}", tabName, tabEntry, exp.Message));
                        MetadataStore[tabName][tabEntry] = "[FILTER ERROR] " + exp.Message;
                    }

                    if (entryShouldBeFiltered)
                        MetadataStore[tabName][tabEntry] = "[FILTERED]";
                }
            }
        }

        /// <summary>
        /// Merge two metadata objects into one. The first metadata object will be returned
        /// with the other metadata's data merged into it.
        /// </summary>
        /// <param name="currentData">The first (base) metadata object to merge</param>
        /// <param name="dataToAdd">The second metadata object to merge</param>
        /// <returns>The merged metadata</returns>
        private static Metadata Merge(Metadata currentData, Metadata dataToAdd)
        {
            if (dataToAdd == null || dataToAdd.MetadataStore.Count == 0)
                return currentData;

            var currStore = currentData.MetadataStore;
            var storeToAdd = dataToAdd.MetadataStore;

            // Loop through all the tabs that are in the data to add...
            foreach (var newTab in storeToAdd)
            {
                // If the tab doesn't exist in current data, add a blank tab
                if (!currStore.ContainsKey(newTab.Key))
                    currStore.Add(newTab.Key, new Dictionary<string, object>());

                var currTab = currStore[newTab.Key];

                foreach (var newTabEntry in newTab.Value)
                {
                    // Only add the entry if its a new tab entry, otherwise overwrite the existing entry
                    if (!currTab.ContainsKey(newTabEntry.Key))
                        currTab.Add(newTabEntry.Key, newTabEntry.Value);
                    else
                        currTab[newTabEntry.Key] = newTabEntry.Value;
                }
            }
            return currentData;
        }
    }
}
