using System;
using System.Collections.Generic;
using System.Linq;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents a single event in a Bugsnag payload.
  /// </summary>
  public class Event : Dictionary<string, object>
  {
    private HandledState _handledState;

    public Event(string payloadVersion, App app, Device device, System.Exception exception, HandledState handledState, IEnumerable<Breadcrumb> breadcrumbs, Session session)
    {
      this.AddToPayload("payloadVersion", payloadVersion);
      this.AddToPayload("exceptions", new Exceptions(exception, 5).ToArray());
      this.AddToPayload("app", app);
      this.AddToPayload("device", device);
      this.AddToPayload("metaData", new Metadata());
      this.AddToPayload("breadcrumbs", breadcrumbs);
      this.AddToPayload("session", session);
      HandledState = handledState;
    }

    public bool IsHandled
    {
      get
      {
        if (this.Get("unhandled") is bool unhandled)
        {
          return !unhandled;
        }

        return false;
      }
    }

    public Exception[] Exceptions
    {
      get { return this.Get("exceptions") as Exception[]; }
      set { this.AddToPayload("exceptions", value); }
    }

    public string Context
    {
      get { return this.Get("context") as string; }
      set { this.AddToPayload("context", value); }
    }

    public string GroupingHash
    {
      get { return this.Get("groupingHash") as string; }
      set { this.AddToPayload("groupingHash", value); }
    }

    public App App
    {
      get { return this.Get("app") as App; }
    }

    public Device Device
    {
      get { return this.Get("device") as Device; }
    }

    public Metadata Metadata
    {
      get { return this.Get("metaData") as Metadata; }
    }

    public User User
    {
      get { return this.Get("user") as User; }
      set { this.AddToPayload("user", value); }
    }

    public Request Request
    {
      get { return this.Get("request") as Request; }
      set
      {
        if (Context == null && value != null)
        {
          if (Uri.TryCreate(value.Url, UriKind.Absolute, out Uri uri))
          {
            Context = uri.AbsolutePath;
          }
          else
          {
            Context = value.Url;
          }
        }

        this.AddToPayload("request", value);
      }
    }

    public IEnumerable<Breadcrumb> Breadcrumbs
    {
      get { return this.Get("breadcrumbs") as IEnumerable<Breadcrumb>; }
    }

    public Severity Severity
    {
      set
      {
        HandledState = HandledState.ForCallbackSpecifiedSeverity(value, _handledState);
      }
      get
      {
        return _handledState.Severity;
      }
    }

    private HandledState HandledState
    {
      set
      {
        _handledState = value;
        foreach (var item in value)
        {
          this[item.Key] = item.Value;
        }
      }
    }

    /// <summary>
    /// Called if the payload size is too large to send, removes data so that the payload
    /// can be sent succesfully.
    /// </summary>
    public bool TrimExtraData()
    {
      var trimmed = false;
      if (Breadcrumbs != null)
      {
        Remove("breadcrumbs");
        trimmed = true;
      }
      if (Metadata != null)
      {
        Metadata.Clear();
        Metadata.Add("notifier", "The serialized payload exceeded the 1MB size limit. Metadata and breadcrumbs have been stripped to make the payload a deliverable size.");
        trimmed = true;
      }
      return trimmed;
    }
  }
}
