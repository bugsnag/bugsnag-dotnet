using System;
using Xunit;

namespace Bugsnag.Tests
{
  /// <summary>
  /// Unit-tests for the automatic selection of notify / session
  /// endpoints in <see cref="Bugsnag.Configuration"/>.
  /// </summary>
  public class EndpointTests
  {
    // ───────────────────────────────────────────────────────────────
    // Test fixtures
    // ───────────────────────────────────────────────────────────────
    private const string NormalKey = "a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d";   // any non-hub API key
    private const string HubKey    = "00000c0ffeebabe0000deadbeef0000";   // 5×0 prefix → hub

    [Fact]
    public void Defaults_Are_Used_For_Normal_ApiKey()
    {
      var cfg = new Configuration(NormalKey);

      Assert.Equal(new Uri(Configuration.DefaultEndpoint),        cfg.Endpoint);
      Assert.Equal(new Uri(Configuration.DefaultSessionEndpoint), cfg.SessionEndpoint);
    }

    [Fact]
    public void Hub_Endpoints_Are_Selected_For_Hub_ApiKey()
    {
      var cfg = new Configuration(HubKey);

      Assert.Equal(new Uri(Configuration.HubEndpoint),        cfg.Endpoint);
      Assert.Equal(new Uri(Configuration.HubSessionEndpoint), cfg.SessionEndpoint);
    }

    [Fact]
    public void Explicit_Endpoints_Override_Automatic_Selection()
    {
      var cfg = new Configuration(HubKey)
      {
        Endpoint        = new Uri("https://notify.example.com"),
        SessionEndpoint = new Uri("https://sessions.example.com")
      };

      Assert.Equal(new Uri("https://notify.example.com"),   cfg.Endpoint);
      Assert.Equal(new Uri("https://sessions.example.com"), cfg.SessionEndpoint);
    }

    [Fact]
    public void Blank_ApiKey_Falls_Back_To_Default_Endpoints()
    {
      var cfg = new Configuration(string.Empty);

      Assert.Equal(new Uri(Configuration.DefaultEndpoint),        cfg.Endpoint);
      Assert.Equal(new Uri(Configuration.DefaultSessionEndpoint), cfg.SessionEndpoint);
    }
  }
}