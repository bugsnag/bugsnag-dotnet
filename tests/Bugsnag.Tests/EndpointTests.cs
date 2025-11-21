using System;
using Xunit;

using CoreConfiguration = global::Bugsnag.Configuration;

namespace Bugsnag.Tests
{
  public class EndpointTests
  {
    // 5 leading zeroes → Secondary
    private const string SecondaryKey = "00000aaaaaaaaaaaaaaaaaaaaaaaaaaa";
    // Any non-secondary key → classic
    private const string ClassicKey = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    [Fact]
    public void ClassicKey_Uses_Bugsnag_Hosts()
    {
      var cfg = new CoreConfiguration(ClassicKey);

      Assert.Equal(new Uri(CoreConfiguration.DefaultEndpoint), cfg.Endpoint);
      Assert.Equal(new Uri(CoreConfiguration.DefaultSessionEndpoint), cfg.SessionEndpoint);
    }

    [Fact]
    public void SecondaryKey_Uses_Smartbear_Hosts()
    {
      var cfg = new CoreConfiguration(SecondaryKey);

      Assert.Equal(new Uri(CoreConfiguration.SecondaryEndpoint), cfg.Endpoint);
      Assert.Equal(new Uri(CoreConfiguration.SecondarySessionEndpoint), cfg.SessionEndpoint);
    }

    [Fact]
    public void SecondaryKey_Honours_Custom_Endpoints_When_Provided()
    {
      var cfg = new CoreConfiguration(SecondaryKey)
      {
        Endpoint = new Uri("https://notify.example.com"),
        SessionEndpoint = new Uri("https://sessions.example.com")
      };

      Assert.Equal(new Uri("https://notify.example.com"), cfg.Endpoint);
      Assert.Equal(new Uri("https://sessions.example.com"), cfg.SessionEndpoint);
    }

    [Fact]
    public void BlankKey_Falls_Back_To_Default_Hosts()
    {
      var cfg = new CoreConfiguration(string.Empty);

      Assert.Equal(new Uri(CoreConfiguration.DefaultEndpoint), cfg.Endpoint);
      Assert.Equal(new Uri(CoreConfiguration.DefaultSessionEndpoint), cfg.SessionEndpoint);
    }
  }
}