using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

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
    public const string HubEndpoint = "https://notify.insighthub.smartbear.com";
    public const string HubSessionEndpoint = "https://sessions.insighthub.smartbear.com";
    private const string HubKeyPrefix = "00000";


    public Configuration() : this(string.Empty)
    {

    }

    public Configuration(string apiKey)
    {
      ApiKey = apiKey;

      bool isHubKey = IsHubKey(apiKey);
      Endpoint = new Uri(isHubKey ? HubEndpoint : DefaultEndpoint);
      SessionEndpoint = new Uri(isHubKey ? HubSessionEndpoint : DefaultSessionEndpoint);

      AutoNotify = true;
      SessionTrackingInterval = TimeSpan.FromSeconds(60);
      MetadataFilters = new[] { "password", "Authorization" };
      MaximumBreadcrumbs = 25;
      ReleaseStage = Environment.GetEnvironmentVariable("BUGSNAG_RELEASE_STAGE");
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

    public Type[] IgnoreClasses { get; set; }

    public KeyValuePair<string, object>[] GlobalMetadata { get; set; }

    public string[] MetadataFilters { get; set; }

    public bool AutoCaptureSessions { get; set; }

    public Uri SessionEndpoint { get; set; }

    public TimeSpan SessionTrackingInterval { get; set; }

    public IWebProxy Proxy { get; set; }

    public int MaximumBreadcrumbs { get; set; }

    private static bool IsHubKey(string key) =>
  !string.IsNullOrEmpty(key) &&
  key.StartsWith(HubKeyPrefix, StringComparison.OrdinalIgnoreCase);
  }
}
