using System.Diagnostics;

namespace Bugsnag
{
  public class Client
  {
    private readonly IConfiguration _configuration;

    private readonly ReportBuilder _reportBuilder;

    private readonly ITransport _transport;

    public Client(IConfiguration configuration) : this(configuration, ThreadQueueTransport.Instance, new ReportBuilder(configuration))
    {

    }

    public Client(IConfiguration configuration, ITransport transport, ReportBuilder reportBuilder)
    {
      _configuration = configuration;
      _transport = transport;
      _reportBuilder = reportBuilder;
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

      var report = _reportBuilder.Generate(exception, severity);

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
