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
  }

  public class SessionEvents : Dictionary<string, int>
  {
    public SessionEvents(int handled, int unhandled)
    {
      this.AddToPayload("handled", handled);
      this.AddToPayload("unhandled", unhandled);
    }
  }
}
