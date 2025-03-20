using Bugsnag.Payload;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Bugsnag
{
  public class SessionsStore
  {
    private static readonly object _instanceLock = new object();
    private static SessionsStore _instance;

    private readonly Dictionary<IConfiguration, Dictionary<string, long>> _store;
    private readonly object _lock;
    private readonly Timer _timer;

    private SessionsStore()
    {
      _store = new Dictionary<IConfiguration, Dictionary<string, long>>();
      _lock = new object();
      _timer = new Timer(SendSessions, new AutoResetEvent(false), TimeSpan.Zero, TimeSpan.FromSeconds(60));
    }

    private void SendSessions(object state)
    {
      Dictionary<IConfiguration, Dictionary<string, long>> sessionData = new Dictionary<IConfiguration, Dictionary<string, long>>();

      lock (_lock)
      {
        // we only care about entries that have session data
        foreach (var item in _store.Where(d => d.Value.Any()))
        {
          sessionData[item.Key] = new Dictionary<string, long>(item.Value);
          _store[item.Key].Clear();
        }
      }

      foreach (var item in sessionData)
      {
        var payload = new BatchedSessions(item.Key, item.Value);
        DefaultDelivery.Instance.Send(payload);
      }
    }

    internal void Stop()
    {
      _timer.Change(Timeout.Infinite, Timeout.Infinite);
      SendSessions(null);
    }

    public static SessionsStore Instance
    {
      get
      {
        lock (_instanceLock)
        {
          if (_instance == null)
          {
            _instance = new SessionsStore();
          }
        }

        return _instance;
      }
    }

    public Session CreateSession(IConfiguration configuration)
    {
      var session = new Session();

      lock (_lock)
      {
        if (!_store.TryGetValue(configuration, out Dictionary<string, long> sessionCounts))
        {
          _store[configuration] = sessionCounts = new Dictionary<string, long>();
        }

        sessionCounts.TryGetValue(session.SessionKey, out long sessionCount);
        sessionCounts[session.SessionKey] = sessionCount + 1;
      }

      return session;
    }
  }
}
