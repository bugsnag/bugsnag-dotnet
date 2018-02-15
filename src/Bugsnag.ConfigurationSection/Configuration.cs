using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace Bugsnag.ConfigurationSection
{
  public class Configuration : System.Configuration.ConfigurationSection, IConfiguration
  {
    private static Configuration _configuration = ConfigurationManager.GetSection("bugsnag") as Configuration;

    public static Configuration Settings
    {
      get { return _configuration; }
    }

    private const string apiKey = "apiKey";

    [ConfigurationProperty(apiKey, IsRequired = true)]
    public string ApiKey
    {
      get { return this[apiKey] as string; }
    }

    private const string appType = "appType";

    [ConfigurationProperty(appType, IsRequired = false)]
    public string AppType
    {
      get
      {
        switch (ElementInformation.Properties[appType].ValueOrigin)
        {
          case PropertyValueOrigin.Inherited:
          case PropertyValueOrigin.SetHere:
            return this[appType] as string;
          default:
            return null;
        }
      }
    }

    private const string appVersion = "appVersion";

    [ConfigurationProperty(appVersion, IsRequired = false)]
    public string AppVersion
    {
      get
      {
        switch (ElementInformation.Properties[appVersion].ValueOrigin)
        {
          case PropertyValueOrigin.Inherited:
          case PropertyValueOrigin.SetHere:
            return this[appVersion] as string;
          default:
            return null;
        }
      }
    }

    private const string endpoint = "endpoint";

    [ConfigurationProperty(endpoint, IsRequired = false, DefaultValue = Bugsnag.Configuration.DefaultEndpoint)]
    private string InternalEndpoint
    {
      get { return this[endpoint] as string; }
    }

    public Uri Endpoint
    {
      get { return new Uri(InternalEndpoint); }
    }

    private const string notifyReleaseStages = "notifyReleaseStages";

    [ConfigurationProperty(notifyReleaseStages, IsRequired = false)]
    private string InternalNotifyReleaseStages
    {
      get
      {
        switch (ElementInformation.Properties[notifyReleaseStages].ValueOrigin)
        {
          case PropertyValueOrigin.Inherited:
          case PropertyValueOrigin.SetHere:
            return this[notifyReleaseStages] as string;
          default:
            return null;
        }
      }
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

    private const string releaseStage = "releaseStage";

    [ConfigurationProperty(releaseStage, IsRequired = false)]
    public string ReleaseStage
    {
      get
      {
        switch (ElementInformation.Properties[releaseStage].ValueOrigin)
        {
          case PropertyValueOrigin.Inherited:
          case PropertyValueOrigin.SetHere:
            return this[releaseStage] as string;
          default:
            return null;
        }
      }
    }

    private const string projectRoots = "projectRoots";

    [ConfigurationProperty(projectRoots, IsRequired = false)]
    private string InternalProjectRoots
    {
      get
      {
        switch (ElementInformation.Properties[projectRoots].ValueOrigin)
        {
          case PropertyValueOrigin.Inherited:
          case PropertyValueOrigin.SetHere:
            return this[projectRoots] as string;
          default:
            return null;
        }
      }
    }

    public string[] ProjectRoots
    {
      get
      {
        if (InternalProjectRoots != null)
        {
          return InternalProjectRoots.Split(',');
        }

        return null;
      }
    }

    private const string projectNamespaces = "projectNamespaces";

    [ConfigurationProperty(projectNamespaces, IsRequired = false)]
    private string InternalProjectNamespaces
    {
      get
      {
        switch (ElementInformation.Properties[projectNamespaces].ValueOrigin)
        {
          case PropertyValueOrigin.Inherited:
          case PropertyValueOrigin.SetHere:
            return this[projectNamespaces] as string;
          default:
            return null;
        }
      }
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

    private const string ignoreClasses = "ignoreClasses";

    [ConfigurationProperty("ignoreClasses", IsRequired = false)]
    private string InternalIgnoreClasses
    {
      get
      {
        switch (ElementInformation.Properties[ignoreClasses].ValueOrigin)
        {
          case PropertyValueOrigin.Inherited:
          case PropertyValueOrigin.SetHere:
            return this[ignoreClasses] as string;
          default:
            return null;
        }
      }
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

    private const string metadataFilters = "metadataFilters";

    [ConfigurationProperty(metadataFilters, IsRequired = false)]
    private string InternalMetadataFilters
    {
      get
      {
        switch (ElementInformation.Properties[metadataFilters].ValueOrigin)
        {
          case PropertyValueOrigin.Inherited:
          case PropertyValueOrigin.SetHere:
            return this[metadataFilters] as string;
          default:
            return null;
        }
      }
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

    private const string metadata = "metadata";

    [ConfigurationProperty(metadata, IsRequired = false)]
    private GlobalMetadataCollection InternalGlobalMetadata
    {
      get { return (GlobalMetadataCollection)this[metadata]; }
    }

    public KeyValuePair<string, string>[] GlobalMetadata
    {
      get
      {
        if (InternalGlobalMetadata.Count > 0)
        {
          return InternalGlobalMetadata.Cast<GlobalMetadataItem>().Select(i => new KeyValuePair<string, string>(i.Key, i.Value)).ToArray();
        }

        return null;
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

    private const string trackSessions = "trackSessions";

    [ConfigurationProperty(trackSessions, IsRequired = false, DefaultValue = true)]
    public bool TrackSessions
    {
      get
      {
        return (bool)this[trackSessions];
      }
    }

    private const string sessionsEndpoint = "sessionsEndpoint";

    [ConfigurationProperty(sessionsEndpoint, IsRequired = false, DefaultValue = Bugsnag.Configuration.DefaultSessionEndpoint)]
    private string InternalSessionEndpoint
    {
      get { return this[sessionsEndpoint] as string; }
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
