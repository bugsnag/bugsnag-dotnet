using System;
using System.Collections.Generic;

namespace Bugsnag
{
  public class Configuration : IConfiguration
  {
    public const string DefaultEndpoint = "https://notify.bugsnag.com";

    public const string DefaultSessionEndpoint = "https://sessions.bugsnag.com";

    public Configuration() : this(string.Empty)
    {

    }

    public Configuration(string apiKey)
    {
      ApiKey = apiKey;
      Endpoint = new Uri(DefaultEndpoint);
      SessionEndpoint = new Uri(DefaultSessionEndpoint);
      SessionTrackingInterval = TimeSpan.FromSeconds(60);
      MetadataFilters = new[] { "password", "Authorization" };
    }

    public string ApiKey { get; set; }

    public Uri Endpoint { get; set; }

    public string ReleaseStage { get; set; }

    public string[] NotifyReleaseStages { get; set; }

    public string AppVersion { get; set; }

    public string AppType { get; set; }

    public string[] ProjectRoots { get; set; }

    public string[] ProjectNamespaces { get; set; }

    public string[] IgnoreClasses { get; set; }

    public KeyValuePair<string, string>[] GlobalMetadata { get; set; }

    public string[] MetadataFilters { get; set; }

    public bool TrackSessions { get; set; }

    public Uri SessionEndpoint { get; set; }

    public TimeSpan SessionTrackingInterval { get; set; }
  }
}
