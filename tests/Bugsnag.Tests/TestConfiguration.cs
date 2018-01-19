using System;

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
  }
}
