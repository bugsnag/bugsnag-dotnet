using System.Diagnostics;

namespace Bugsnag
{
  public class Client
  {
    private readonly IConfiguration _configuration;

    private readonly ReportFactory _reportFactory;

    private readonly ITransport _transport;

    public Client(IConfiguration configuration) : this(configuration, ThreadQueueTransport.Instance, new ReportFactory(configuration))
    {

    }

    public Client(IConfiguration configuration, ITransport transport, ReportFactory reportFactory)
    {
      _configuration = configuration;
      _transport = transport;
      _reportFactory = reportFactory;
    }

    public IConfiguration Configuration { get { return _configuration; } }

    public void Notify(System.Exception exception)
    {
      Notify(exception, Severity.Error);
    }

    public void Notify(System.Exception exception, Severity severity)
    {
      if (Configuration.InvalidReleaseStage())
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
        _transport.Send(Configuration.Endpoint, rawPayload);
      }
    }
  }
}
