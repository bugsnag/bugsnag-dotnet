using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternalMetaData = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, object>>;

namespace Bugsnag
{
    public class MetaData
    {
        private InternalMetaData metaDataStore;

        public MetaData()
        {
            metaDataStore = new InternalMetaData();
        }

        public void AddToTab(string tabEntryKey, object tabEntryValue)
        {
            AddToTab("Custom Data", tabEntryKey, tabEntryValue);
        }

        public void AddToTab(string tabName, string tabEntryKey, object tabEntryValue)
        {
            if (metaDataStore.ContainsKey(tabName))
                metaDataStore[tabName].Add(tabEntryKey, tabEntryValue);
            else
                metaDataStore.Add(tabName, new Dictionary<string, object>{{tabEntryKey, tabEntryValue}});
        }


        public static InternalMetaData GenerateMetaDataOutput(MetaData data1, MetaData data2)
        {
            var result = new InternalMetaData();

            foreach (var tab in data1.metaDataStore)
            {
                var tabData = new Dictionary<string,object>();
                foreach (var entry in tab.Value)
                {
                    tabData.Add(entry.Key, entry.Value);
                }
                result.Add(tab.Key, tabData);
            }

            foreach (var tab in data2.metaDataStore)
            {
                if (result.ContainsKey(tab.Key))
                {
                    foreach(var entry in tab.Value)
                    {
                        result[tab.Key].Add(entry.Key, entry.Value);
                    }
                }
                else
                {
                    var tabData = new Dictionary<string, object>();
                    foreach (var entry in tab.Value)
                    {
                        tabData.Add(entry.Key, entry.Value);
                    }
                    result.Add(tab.Key, tabData);
                }
            }
            return result;
        }
    }
}
