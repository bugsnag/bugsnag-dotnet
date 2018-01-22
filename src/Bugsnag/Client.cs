using System.Diagnostics;

namespace Bugsnag
{
  public class Client
  {
    private readonly IConfiguration _configuration;

    private readonly ITransport _transport;

    private static Middleware[] InternalMiddleware = new Middleware[] {
      Bugsnag.InternalMiddleware.ReleaseStageFilter,
      Bugsnag.InternalMiddleware.RemoveIgnoredExceptions,
      Bugsnag.InternalMiddleware.RemoveFilePrefixes,
      Bugsnag.InternalMiddleware.DetectInProjectNamespaces,
    }; 

    public Client(IConfiguration configuration) : this(configuration, ThreadQueueTransport.Instance)
    {

    }

    public Client(IConfiguration configuration, ITransport transport)
    {
      _configuration = configuration;
      _transport = transport;
    }

    public IConfiguration Configuration { get { return _configuration; } }

    public void Notify(System.Exception exception)
    {
      Notify(exception, Severity.Error);
    }

    public void Notify(System.Exception exception, Severity severity)
    {
      var report = new Report(_configuration, exception, severity);

      Notify(report);
    }

    public void Notify(Report report)
    {
      foreach (var middleware in InternalMiddleware)
      {
        middleware(Configuration, report);
      }

      if (report.Deliver)
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
}
