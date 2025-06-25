using System;
using Xunit;

using CoreConfiguration = global::Bugsnag.Configuration;

namespace Bugsnag.Tests
{
  public class EndpointTests
  {
    // 5 leading zeroes → Insight Hub
    private const string HubKey    = "00000cafebabefeed0000deadbeef0000";
    // Any non-hub key → classic Bugsnag
    private const string ClassicKey = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    [Fact]
    public void ClassicKey_Uses_Bugsnag_Hosts()
    {
      var cfg = new CoreConfiguration(ClassicKey);

      Assert.Equal(new Uri(CoreConfiguration.DefaultEndpoint),        cfg.Endpoint);
      Assert.Equal(new Uri(CoreConfiguration.DefaultSessionEndpoint), cfg.SessionEndpoint);
    }

    [Fact]
    public void HubKey_Uses_InsightHub_Hosts()
    {
      var cfg = new CoreConfiguration(HubKey);

      Assert.Equal(new Uri(CoreConfiguration.HubEndpoint),        cfg.Endpoint);
      Assert.Equal(new Uri(CoreConfiguration.HubSessionEndpoint), cfg.SessionEndpoint);
    }

    [Fact]
    public void HubKey_Honours_Custom_Endpoints_When_Provided()
    {
      var cfg = new CoreConfiguration(HubKey)
      {
        Endpoint        = new Uri("https://notify.example.com"),
        SessionEndpoint = new Uri("https://sessions.example.com")
      };

      Assert.Equal(new Uri("https://notify.example.com"),   cfg.Endpoint);
      Assert.Equal(new Uri("https://sessions.example.com"), cfg.SessionEndpoint);
    }

    [Fact]
    public void BlankKey_Falls_Back_To_Default_Hosts()
    {
      var cfg = new CoreConfiguration(string.Empty);

      Assert.Equal(new Uri(CoreConfiguration.DefaultEndpoint),        cfg.Endpoint);
      Assert.Equal(new Uri(CoreConfiguration.DefaultSessionEndpoint), cfg.SessionEndpoint);
    }
  }
}