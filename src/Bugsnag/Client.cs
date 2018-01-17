using System;
using System.Diagnostics;
using System.Linq;

namespace Bugsnag
{
  public class Client
  {
    private readonly ReportFactory _reportFactory;

    private readonly ITransport _transport;

    private readonly Uri _endpoint;

    private readonly string _releaseStage;

    private readonly string[] _notifyReleaseStages;

    public Client(IConfiguration configuration) : this(configuration, ThreadQueueTransport.Instance, new ReportFactory(configuration))
    {

    }

    public Client(IConfiguration configuration, ITransport transport, ReportFactory reportFactory)
    {
      _endpoint = configuration.Endpoint;
      _releaseStage = configuration.ReleaseStage;
      _notifyReleaseStages = configuration.NotifyReleaseStages;
      _transport = transport;
      _reportFactory = reportFactory;
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
        _transport.Send(_endpoint, rawPayload);
      }
    }
  }
}
