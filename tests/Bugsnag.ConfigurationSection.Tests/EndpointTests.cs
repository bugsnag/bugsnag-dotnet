using System;
using System.Configuration;
using Xunit;

namespace Bugsnag.ConfigurationSection.Tests
{
  /// <summary>
  /// Verifies that a “Hub”-flavoured API-key (prefix 00000…)
  /// automatically switches the notify / sessions hosts to InsightHub,
  /// *unless* the user deliberately overrides them in the XML.
  /// </summary>
  public class HubEndpointTests
  {
    private static IConfiguration Load(string cfgName)
    {
      // all .config test files sit next to the binaries
      var map = new ExeConfigurationFileMap { ExeConfigFilename = $".\\{cfgName}.config" };
      return ConfigurationManager
               .OpenMappedExeConfiguration(map, ConfigurationUserLevel.None)
               .GetSection("bugsnag") as Configuration;
    }

    /*─────────────────────────────────────────────────────────
     * 1. Hub key, *default* endpoints  →  InsightHub hosts
     *────────────────────────────────────────────────────────*/
    [Fact]
    public void HubKey_UsesInsightHubHosts_WhenNotOverridden()
    {
      var cfg = Load("HubDefault");   // see HubDefault.config below

      Assert.NotNull(cfg);
      Assert.Equal("00000123456789abcdef0123456789", cfg.ApiKey);

      Assert.Equal(new Uri("https://notify.insighthub.smartbear.com"),   cfg.Endpoint);
      Assert.Equal(new Uri("https://sessions.insighthub.smartbear.com"), cfg.SessionEndpoint);
    }

    /*─────────────────────────────────────────────────────────
     * 2. Hub key but custom <endpoint> / <sessionsEndpoint>   →
     *    **user preference wins**
     *────────────────────────────────────────────────────────*/
    [Fact]
    public void HubKey_HonoursCustomEndpoints_WhenPresent()
    {
      var cfg = Load("HubCustomEndpoint");   // see HubCustomEndpoint.config below

      Assert.NotNull(cfg);
      Assert.Equal("00000feedfacecafebeefdeadbeef", cfg.ApiKey);

      Assert.Equal(new Uri("https://corp.example.com/notify"),   cfg.Endpoint);
      Assert.Equal(new Uri("https://corp.example.com/sessions"), cfg.SessionEndpoint);
    }

    /*─────────────────────────────────────────────────────────
     * 3. Non-hub key ⇒ still Bugsnag hosts
     *────────────────────────────────────────────────────────*/
    [Fact]
    public void ClassicKey_KeepsBugsnagHosts()
    {
      var cfg = Load("ClassicDefault");   // see ClassicDefault.config below

      Assert.NotNull(cfg);
      Assert.Equal("abcd1234abcd1234abcd1234abcd1234", cfg.ApiKey);

      Assert.Equal(new Uri(Bugsnag.Configuration.DefaultEndpoint),        cfg.Endpoint);
      Assert.Equal(new Uri(Bugsnag.Configuration.DefaultSessionEndpoint), cfg.SessionEndpoint);
    }
  }
}