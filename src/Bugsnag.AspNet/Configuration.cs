using System;
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

    [ConfigurationProperty("endpoint", IsRequired = false)]
    public Uri Endpoint
    {
      get { return this["endpoint"] as Uri; }
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

    [ConfigurationProperty("projectNamespaces", IsRequired = false)]
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
  }
}
