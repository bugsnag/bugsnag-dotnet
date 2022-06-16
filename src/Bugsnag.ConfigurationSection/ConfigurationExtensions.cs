namespace Bugsnag.ConfigurationSection
{
  public static class ConfigurationExtensions
  {
    public static Bugsnag.Configuration ToWritableCopy(this Configuration config)
    {
      return new Bugsnag.Configuration()
      {
        ApiKey = config.ApiKey,
        AppType = config.AppType,
        AppVersion = config.AppVersion,
        AutoCaptureSessions = config.AutoCaptureSessions,
        AutoNotify = config.AutoNotify,
        Endpoint = config.Endpoint,
        GlobalMetadata = config.GlobalMetadata,
        IgnoreClasses = config.IgnoreClasses,
        MaximumBreadcrumbs = config.MaximumBreadcrumbs,
        MetadataFilters = config.MetadataFilters,
        NotifyReleaseStages = config.NotifyReleaseStages,
        ProjectNamespaces = config.ProjectNamespaces,
        ProjectRoots = config.ProjectRoots,
        Proxy = config.Proxy,
        ReleaseStage = config.ReleaseStage,
        SessionEndpoint = config.SessionEndpoint,
        SessionTrackingInterval = config.SessionTrackingInterval
      };
    }
  }
}
