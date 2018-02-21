using Bugsnag.SessionTracking;

namespace Bugsnag
{
  public static class Singleton
  {
    private static IClient _client;

    public static void ConfigureClient(IConfiguration configuration)
    {
      ConfigureClient(configuration, ThreadQueueTransport.Instance, new InMemoryBreadcrumbs(), new InMemorySessionTracker(configuration));
    }

    public static void ConfigureClient(IConfiguration configuration, ITransport transport, IBreadcrumbs breadcrumbs, ISessionTracker sessionTracker)
    {
      _client = new Client(configuration, transport, breadcrumbs, sessionTracker);
    }

    public static IClient Client => _client;
  }
}
