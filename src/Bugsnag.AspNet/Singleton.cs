namespace Bugsnag.AspNet
{
  public static class Singleton
  {
    public static void ConfigureClientWithWebConfig()
    {
      ConfigureClient(ConfigurationSection.Configuration.Settings);
    }

    public static void ConfigureClient(IConfiguration configuration)
    {
      Bugsnag.Singleton.ConfigureClient(configuration, ThreadQueueTransport.Instance, new HttpContextBreadcrumbs(), new HttpContextSessionTracker(configuration));
    }
  }
}
