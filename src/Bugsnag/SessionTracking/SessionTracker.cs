using Bugsnag.Payload;

namespace Bugsnag.SessionTracking
{
  public abstract class SessionTracker
  {
    private readonly IConfiguration _configuration;

    public SessionTracker(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public void CreateSession()
    {
      CurrentSession = SessionsStore.Instance.CreateSession(_configuration);
    }

    public abstract Session CurrentSession { get; protected set; }
  }
}
