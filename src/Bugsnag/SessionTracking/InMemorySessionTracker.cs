using Bugsnag.Payload;

namespace Bugsnag.SessionTracking
{
  public class InMemorySessionTracker : SessionTracker
  {
    private Session _session;

    public InMemorySessionTracker(IConfiguration configuration) : base(configuration)
    {

    }

    public override Session CurrentSession { get => _session; protected set => _session = value; }
  }
}
