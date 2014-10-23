using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bugsnag.Message.Core
{
    [JsonConverter(typeof(TabSerializer))]
    public class TabInfo
    {
        public List<TabEntry> Entries{get;set;}
    }

    public abstract class TabEntry { }

    public class TabSection : TabEntry
    {
        public string Name { get; set; }
        public List<TabKeyValuePair> Pairs {get;set;}
    }

    public class TabKeyValuePair : TabEntry
    {
        public KeyValuePair<string,string> Pair {get;set;}

        public TabKeyValuePair(string key, string value)
        {
            Pair = new KeyValuePair<string, string>(key, value);
        }
    }

    public class TabSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var name = value as TabInfo;
            writer.WriteStartObject();
            foreach(var entry in name.Entries)
            {
                if (entry is TabKeyValuePair)
                {
                    var e = entry as TabKeyValuePair;
                    writer.WritePropertyName(e.Pair.Key);
                    writer.WriteValue(e.Pair.Value);
                }
                else
                {
                    var sec = entry as TabSection;
                    writer.WritePropertyName(sec.Name);
                    writer.WriteStartObject();
                    foreach(var e in sec.Pairs)
                    {
                        writer.WritePropertyName(e.Pair.Key);
                        writer.WriteValue(e.Pair.Value);
                    }
                    writer.WriteEndObject();
                }
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TabInfo).IsAssignableFrom(objectType);
        }
    }
}
