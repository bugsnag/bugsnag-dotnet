using Bugsnag.Payload;

namespace Bugsnag.SessionTracking
{
  public interface ISessionTracker
  {
    void CreateSession();

    Session CurrentSession { get; }
  }

  public abstract class SessionTracker : ISessionTracker
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
