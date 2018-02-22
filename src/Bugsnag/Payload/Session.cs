using System;
using System.Collections.Generic;

namespace Bugsnag.Payload
{
  public class Session : Dictionary<string, object>
  {
    public Session() : this(DateTime.UtcNow, 0, 0)
    {

    }

    public Session(DateTime startedAt, int handled, int unhandled)
    {
      this.AddToPayload("id", Guid.NewGuid());
      this.AddToPayload("startedAt", startedAt);
      this.AddToPayload("events", new SessionEvents(handled, unhandled));
    }

    public string SessionKey
    {
      get
      {
        return ((DateTime)this.Get("startedAt")).ToString("yyyy-MM-ddTHH:mm:00");
      }
    }

    public void AddException(Report report)
    {
      if (this.Get("events") is SessionEvents sessions)
      {
        foreach (var @event in report.Events)
        {
          if (@event.IsHandled)
          {
            sessions.IncrementHandledCount();
          }
          else
          {
            sessions.IncrementUnhandledCount();
          }
        }
      }
    }
  }

  public class SessionEvents : Dictionary<string, int>
  {
    private readonly object _handledLock = new object();
    private readonly object _unhandledLock = new object();

    public SessionEvents(int handled, int unhandled)
    {
      this.AddToPayload("handled", handled);
      this.AddToPayload("unhandled", unhandled);
    }

    public void IncrementHandledCount()
    {
      lock (_handledLock)
      {
        this["handled"]++;
      }
    }

    public void IncrementUnhandledCount()
    {
      lock (_unhandledLock)
      {
        this["unhandled"]++;
      }
    }
  }
}
