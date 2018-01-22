using System;

namespace Bugsnag
{
  public interface IConfiguration
  {
    string ApiKey { get; }

    Uri Endpoint { get; }

    string ReleaseStage { get; }

    string[] NotifyReleaseStages { get; }

    string AppVersion { get; }

    string AppType { get; }

    string[] FilePrefixes { get; }

    string[] ProjectNamespaces { get; }

    string[] IgnoreClasses { get; }
  }
}
