using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bugsnag
{
  public class Client
  {
    private readonly ReportFactory _reportFactory;

    private readonly Transport _transport;

    private readonly Uri _endpoint;

    private readonly string _releaseStage;

    private readonly string[] _notifyReleaseStages;

    private readonly Dictionary<IAsyncResult, bool> _inflight;

    public Client(IConfiguration configuration) : this(configuration, new Transport(), new ReportFactory(configuration))
    {

    }

    public Client(IConfiguration configuration, Transport transport, ReportFactory reportFactory)
    {
      _endpoint = configuration.Endpoint;
      _releaseStage = configuration.ReleaseStage;
      _notifyReleaseStages = configuration.NotifyReleaseStages;
      _transport = transport;
      _reportFactory = reportFactory;
      _inflight = new Dictionary<IAsyncResult, bool>();
    }

    public void Notify(System.Exception exception)
    {
      Notify(exception, Severity.Error);
    }

    public void Notify(System.Exception exception, Severity severity)
    {
      if (_notifyReleaseStages != null && !string.IsNullOrEmpty(_releaseStage) && !_notifyReleaseStages.Any(stage => stage == _releaseStage))
      {
        return;
      }

      var report = _reportFactory.Generate(exception, severity);

      Notify(report);
    }

    public void Notify(Report report)
    {
      byte[] rawPayload = null;

      try
      {
        var payload = SimpleJson.SimpleJson.SerializeObject(report);
        rawPayload = System.Text.Encoding.UTF8.GetBytes(payload);
      }
      catch (System.Exception exception)
      {
        Trace.WriteLine(exception);
      }

      if (rawPayload != null)
      {
        var asyncResult = _transport.BeginSend(_endpoint, rawPayload, ReportCallback, new ClientState(report, rawPayload));
        _inflight.Add(asyncResult, true);
      }
    }

    private void ReportCallback(IAsyncResult asyncResult)
    {
      var state = (ClientState)asyncResult.AsyncState;
      var responseCode = _transport.EndSend(asyncResult);
      // don't do anything with the result right now
      _inflight.Remove(asyncResult);
    }

    private class ClientState
    {
      public Report Report { get; private set; }

      public byte[] Payload { get; private set; }

      public ClientState(Report report, byte[] payload)
      {
        Report = report;
        Payload = payload;
      }
    }
  }
}
