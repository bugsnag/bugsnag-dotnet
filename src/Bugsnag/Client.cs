using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bugsnag
{
  public class Client
  {
    private readonly NotificationFactory _notificationFactory;

    private readonly Transport _transport;

    private readonly Uri _endpoint;

    private readonly string _releaseStage;

    private readonly string[] _notifyReleaseStages;

    private readonly Dictionary<IAsyncResult, bool> _inflight;

    public Client(IConfiguration configuration) : this(configuration, new Transport(), new NotificationFactory(configuration))
    {

    }

    public Client(IConfiguration configuration, Transport transport, NotificationFactory notificationFactory)
    {
      _endpoint = configuration.Endpoint;
      _releaseStage = configuration.ReleaseStage;
      _notifyReleaseStages = configuration.NotifyReleaseStages;
      _transport = transport;
      _notificationFactory = notificationFactory;
      _inflight = new Dictionary<IAsyncResult, bool>();
    }

    public void Notify(Exception exception)
    {
      Notify(exception, Severity.Error);
    }

    public void Notify(Exception exception, Severity severity)
    {
      if (_notifyReleaseStages != null && !string.IsNullOrEmpty(_releaseStage) && !_notifyReleaseStages.Any(stage => stage == _releaseStage))
      {
        return;
      }

      var notification = _notificationFactory.Generate(exception, severity);

      Notify(notification);
    }

    public void Notify(Payload.Notification notification)
    {
      byte[] rawPayload = null;

      try
      {
        var payload = SimpleJson.SimpleJson.SerializeObject(notification);
        rawPayload = System.Text.Encoding.UTF8.GetBytes(payload);
      }
      catch (Exception exception)
      {
        Trace.WriteLine(exception);
      }

      if (rawPayload != null)
      {
        var asyncResult = _transport.BeginSend(_endpoint, rawPayload, NotificationCallback, new ClientState(notification, rawPayload));
        _inflight.Add(asyncResult, true);
      }
    }

    private void NotificationCallback(IAsyncResult asyncResult)
    {
      var state = (ClientState)asyncResult.AsyncState;
      var responseCode = _transport.EndSend(asyncResult);
      // don't do anything with the result right now
      _inflight.Remove(asyncResult);
    }

    private class ClientState
    {
      public Payload.Notification Notification { get; private set; }

      public byte[] Payload { get; private set; }

      public ClientState(Payload.Notification notification, byte[] payload)
      {
        Notification = notification;
        Payload = payload;
      }
    }
  }
}
