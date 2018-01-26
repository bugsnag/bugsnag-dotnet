using System;
using System.Collections.Generic;

namespace Bugsnag.Tests
{
  class TestConfiguration : IConfiguration
  {
    public string ApiKey => "123456";

    public Uri Endpoint => new Uri("https://notify.bugsnag.com");

    public string ReleaseStage => "test";

    public string[] NotifyReleaseStages => null;

    public string AppVersion => "1.0";

    public string AppType => "test";

    public string[] FilePrefixes => new string[] { };

    public string[] ProjectNamespaces => new string[] { };

    public string[] IgnoreClasses => new string[] { };

    public IEnumerable<KeyValuePair<string, string>> GlobalMetadata => new KeyValuePair<string, string>[] { };
  }
}
