using System;
using System.Configuration;
using Xunit;

namespace Bugsnag.ConfigurationSection.Tests
{
  /// <summary>
  /// Verifies that a Secondary-flavoured API-key (prefix 00000…)
  /// automatically switches the notify / sessions hosts to smartbear.com,
  /// *unless* the user deliberately overrides them in the XML.
  /// </summary>
  public class SecondaryEndpointTests
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
     * 1. Secondary key, *default* endpoints  →  smartbear.com hosts
     *────────────────────────────────────────────────────────*/
    [Fact]
    public void SecondaryKey_UsesSmartbearHosts_WhenNotOverridden()
    {
      var cfg = Load("SecondaryDefault");   // see SecondaryDefault.config below

      Assert.NotNull(cfg);
      Assert.Equal("00000123456789abcdef0123456789", cfg.ApiKey);

      Assert.Equal(new Uri("https://notify.bugsnag.smartbear.com"), cfg.Endpoint);
      Assert.Equal(new Uri("https://sessions.bugsnag.smartbear.com"), cfg.SessionEndpoint);
    }

    /*─────────────────────────────────────────────────────────
     * 2. Secondary key but custom <endpoint> / <sessionsEndpoint>   →
     *    **user preference wins**
     *────────────────────────────────────────────────────────*/
    [Fact]
    public void SecondaryKey_HonoursCustomEndpoints_WhenPresent()
    {
      var cfg = Load("SecondaryEndpoint");   // see SecondaryEndpoint.config below

      Assert.NotNull(cfg);
      Assert.Equal("00000feedfacecafebeefdeadbeef", cfg.ApiKey);

      Assert.Equal(new Uri("https://corp.example.com/notify"), cfg.Endpoint);
      Assert.Equal(new Uri("https://corp.example.com/sessions"), cfg.SessionEndpoint);
    }

    /*─────────────────────────────────────────────────────────
     * 3. Non-secondary key ⇒ still Bugsnag hosts
     *────────────────────────────────────────────────────────*/
    [Fact]
    public void ClassicKey_KeepsBugsnagHosts()
    {
      var cfg = Load("ClassicDefault");   // see ClassicDefault.config below

      Assert.NotNull(cfg);
      Assert.Equal("abcd1234abcd1234abcd1234abcd1234", cfg.ApiKey);

      Assert.Equal(new Uri(Bugsnag.Configuration.DefaultEndpoint), cfg.Endpoint);
      Assert.Equal(new Uri(Bugsnag.Configuration.DefaultSessionEndpoint), cfg.SessionEndpoint);
    }
  }
}