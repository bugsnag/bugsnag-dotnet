using System;
using System.Collections.Generic;
using System.Net;

namespace Bugsnag
{
  /// <summary>
  /// An in memory implementation of the IConfiguration interface, with default
  /// values populated.
  /// </summary>
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
      AutoNotify = true;
      SessionEndpoint = new Uri(DefaultSessionEndpoint);
      SessionTrackingInterval = TimeSpan.FromSeconds(60);
      MetadataFilters = new[] { "password", "Authorization" };
      MaximumBreadcrumbs = 25;
    }

    public string ApiKey { get; set; }

    public Uri Endpoint { get; set; }

    public bool AutoNotify { get; set; }

    public string ReleaseStage { get; set; }

    public string[] NotifyReleaseStages { get; set; }

    public string AppVersion { get; set; }

    public string AppType { get; set; }

    public string[] ProjectRoots { get; set; }

    public string[] ProjectNamespaces { get; set; }

    public string[] IgnoreClasses { get; set; }

    public KeyValuePair<string, string>[] GlobalMetadata { get; set; }

    public string[] MetadataFilters { get; set; }

    public bool AutoCaptureSessions { get; set; }

    public Uri SessionEndpoint { get; set; }

    public TimeSpan SessionTrackingInterval { get; set; }

    public IWebProxy Proxy { get; set; }

    public int MaximumBreadcrumbs { get; set; }
  }
}
