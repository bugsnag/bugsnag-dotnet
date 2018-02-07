using Bugsnag.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Bugsnag
{
  public abstract class SessionTracking
  {
    private static Dictionary<string, long> _sessionStore = new Dictionary<string, long>();
    private static readonly object _sessionsLock = new object();
    private readonly Timer _timer;
    private readonly ITransport _transport;
    private readonly IConfiguration _configuration;

    public SessionTracking(IConfiguration configuration) : this(configuration, ThreadQueueTransport.Instance)
    {

    }

    public SessionTracking(IConfiguration configuration, ITransport transport)
    {
      _configuration = configuration;
      _transport = transport;
      _timer = new Timer(SendSessions, new AutoResetEvent(false), TimeSpan.Zero, configuration.SessionTrackingInterval);
    }

    private Dictionary<string, long> SessionStore { get { return _sessionStore; } }

    private void SendSessions(object state)
    {
      Dictionary<string, long> sessionData;

      lock (_sessionsLock)
      {
        sessionData = new Dictionary<string, long>(SessionStore);
        SessionStore.Clear();
      }

      if (sessionData.Any())
      {
        var batchedSessionsPayload = new BatchedSessions(_configuration, sessionData);
        var rawPayload = batchedSessionsPayload.Serialize();

        if (rawPayload != null)
        {
          _transport.Send(batchedSessionsPayload);
        }
      }
    }

    public void CreateSession()
    {
      lock (_sessionsLock)
      {
        if (CurrentSession == null)
        {
          CurrentSession = new Session();
        }

        SessionStore.TryGetValue(CurrentSession.SessionKey, out long sessionCount);
        SessionStore[CurrentSession.SessionKey] = sessionCount + 1;
      }
    }

    public abstract Session CurrentSession { get; protected set; }
  }

  public class InMemorySessionTracking : SessionTracking
  {
    private Session _session;

    public InMemorySessionTracking(IConfiguration configuration) : base(configuration)
    {

    }

    public override Session CurrentSession { get => _session; protected set => _session = value; }
  }

  public class ThreadStaticSessionTracking : SessionTracking
  {
    [ThreadStatic]
    private Session _currentSession;

    public ThreadStaticSessionTracking(IConfiguration configuration) : base(configuration)
    {

    }

    public override Session CurrentSession { get => _currentSession; protected set => _currentSession = value; }
  }
}
