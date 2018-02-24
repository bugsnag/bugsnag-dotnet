using Bugsnag.Payload;

namespace Bugsnag
{
  public interface ISessionTracker
  {
    void CreateSession();

    Session CurrentSession { get; }
  }

  public class SessionTracker : ISessionTracker
  {
    private readonly IConfiguration _configuration;
    private Session _currentSession;

    public SessionTracker(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public Session CurrentSession => _currentSession;

    public void CreateSession()
    {
      _currentSession = SessionsStore.Instance.CreateSession(_configuration);
    }
  }
}
