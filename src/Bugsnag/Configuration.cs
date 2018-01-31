using System;
using System.Collections.Generic;

namespace Bugsnag
{
  public class Configuration : IConfiguration
  {
    public const string DefaultEndpoint = "https://notify.bugsnag.com";

    public Configuration(string apiKey)
    {
      ApiKey = apiKey;
      Endpoint = new Uri(DefaultEndpoint);
    }

    public string ApiKey { get; set; }

    public Uri Endpoint { get; set; }

    public string ReleaseStage { get; set; }

    public string[] NotifyReleaseStages { get; set; }

    public string AppVersion { get; set; }

    public string AppType { get; set; }

    public string[] FilePrefixes { get; set; }

    public string[] ProjectNamespaces { get; set; }

    public string[] IgnoreClasses { get; set; }

    public IEnumerable<KeyValuePair<string, string>> GlobalMetadata { get; set; }
  }
}
