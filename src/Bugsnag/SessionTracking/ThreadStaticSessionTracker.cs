using Bugsnag.Payload;
using System;

namespace Bugsnag.SessionTracking
{
  public class ThreadStaticSessionTracker : SessionTracker
  {
    [ThreadStatic]
    private Session _currentSession;

    public ThreadStaticSessionTracker(IConfiguration configuration) : base(configuration)
    {

    }

    public override Session CurrentSession { get => _currentSession; protected set => _currentSession = value; }
  }
}
