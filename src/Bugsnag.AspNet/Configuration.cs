using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace Bugsnag.AspNet
{
  public class Configuration : ConfigurationSection, IConfiguration
  {
    private static Configuration _configuration = ConfigurationManager.GetSection("bugsnag") as Configuration;

    public static Configuration Settings
    {
      get { return _configuration; }
    }

    [ConfigurationProperty("apiKey", IsRequired = true)]
    public string ApiKey
    {
      get { return this["apiKey"] as string; }
    }

    [ConfigurationProperty("appType", IsRequired = false)]
    public string AppType
    {
      get { return this["appType"] as string; }
    }

    [ConfigurationProperty("appVersion", IsRequired = false)]
    public string AppVersion
    {
      get { return this["appVersion"] as string; }
    }

    [ConfigurationProperty("endpoint", IsRequired = false, DefaultValue = Bugsnag.Configuration.DefaultEndpoint)]
    private string InternalEndpoint
    {
      get { return this["endpoint"] as string; }
    }

    public Uri Endpoint
    {
      get { return new Uri(InternalEndpoint); }
    }

    [ConfigurationProperty("notifyReleaseStages", IsRequired = false)]
    private string InternalNotifyReleaseStages
    {
      get { return this["notifyReleaseStages"] as string; }
    }

    public string[] NotifyReleaseStages
    {
      get
      {
        if (InternalNotifyReleaseStages != null)
        {
          return InternalNotifyReleaseStages.Split(',');
        }

        return null;
      }
    }

    [ConfigurationProperty("releaseStage", IsRequired = false)]
    public string ReleaseStage
    {
      get { return this["releaseStage"] as string; }
    }

    [ConfigurationProperty("filePrefixes", IsRequired = false)]
    private string InternalFilePrefixes
    {
      get { return this["filePrefixes"] as string; }
    }

    public string[] FilePrefixes
    {
      get
      {
        if (InternalFilePrefixes != null)
        {
          return InternalFilePrefixes.Split(',');
        }

        return null;
      }
    }

    [ConfigurationProperty("projectNamespaces", IsRequired = false)]
    private string InternalProjectNamespaces
    {
      get { return this["projectNamespaces"] as string; }
    }

    public string[] ProjectNamespaces
    {
      get
      {
        if (InternalProjectNamespaces != null)
        {
          return InternalProjectNamespaces.Split(',');
        }

        return null;
      }
    }

    [ConfigurationProperty("ignoreClasses", IsRequired = false)]
    private string InternalIgnoreClasses
    {
      get { return this["ignoreClasses"] as string; }
    }

    public string[] IgnoreClasses
    {
      get
      {
        if (InternalIgnoreClasses != null)
        {
          return InternalIgnoreClasses.Split(',');
        }

        return null;
      }
    }

    [ConfigurationProperty("metadataFilters", IsRequired = false)]
    private string InternalMetadataFilters
    {
      get { return this["metadataFilters"] as string; }
    }

    public string[] MetadataFilters
    {
      get
      {
        if (InternalMetadataFilters != null)
        {
          return InternalMetadataFilters.Split(',');
        }

        return null;
      }
    }

    [ConfigurationProperty("metadata", IsRequired = false)]
    private GlobalMetadataCollection InternalGlobalMetadata
    {
      get { return (GlobalMetadataCollection)this["metadata"]; }
    }

    public KeyValuePair<string, string>[] GlobalMetadata
    {
      get
      {
        return InternalGlobalMetadata.Cast<GlobalMetadataItem>().Select(i => new KeyValuePair<string, string>(i.Key, i.Value)).ToArray();
      }
    }

    class GlobalMetadataItem : ConfigurationElement
    {
      [ConfigurationProperty("key", IsRequired = true)]
      public string Key { get { return (string)this["key"]; } }

      [ConfigurationProperty("value", IsRequired = true)]
      public string Value { get { return (string)this["value"]; } }
    }

    [ConfigurationCollection(typeof(GlobalMetadataItem), AddItemName = "item", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    class GlobalMetadataCollection : ConfigurationElementCollection
    {
      protected override ConfigurationElement CreateNewElement()
      {
        return new GlobalMetadataItem();
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
        return ((GlobalMetadataItem)element).Key;
      }
    }

    [ConfigurationProperty("trackSessions", IsRequired = false, DefaultValue = true)]
    public bool TrackSessions
    {
      get
      {
        return (bool)this["trackSessions"];
      }
    }

    [ConfigurationProperty("sessionsEndpoint", IsRequired = false, DefaultValue = Bugsnag.Configuration.DefaultSessionEndpoint)]
    private string InternalSessionEndpoint
    {
      get { return this["sessionsEndpoint"] as string; }
    }

    public Uri SessionEndpoint
    {
      get { return new Uri(InternalSessionEndpoint); }
    }

    public TimeSpan SessionTrackingInterval
    {
      get { return TimeSpan.FromSeconds(60); }
    }
  }
}
