using System.Collections.Generic;
using System.Diagnostics;

namespace Bugsnag
{
  public class Client
  {
    private readonly IConfiguration _configuration;

    private readonly ITransport _transport;

    private readonly IStorage<Breadcrumb> _breadcrumbStore;

    private static Middleware[] InternalMiddleware = new Middleware[] {
      Bugsnag.InternalMiddleware.ReleaseStageFilter,
      Bugsnag.InternalMiddleware.RemoveIgnoredExceptions,
      Bugsnag.InternalMiddleware.RemoveFilePrefixes,
      Bugsnag.InternalMiddleware.DetectInProjectNamespaces,
    };

    public Client(IConfiguration configuration) : this(configuration, ThreadQueueTransport.Instance, new InMemoryStorage<Breadcrumb>())
    {

    }

    public Client(IConfiguration configuration, ITransport transport, IStorage<Breadcrumb> breadcrumbStore)
    {
      _configuration = configuration;
      _transport = transport;
      _breadcrumbStore = breadcrumbStore;
    }

    public IConfiguration Configuration { get { return _configuration; } }

    protected IStorage<Breadcrumb> BreadcrumbStore {  get { return _breadcrumbStore; } }

    public void Notify(System.Exception exception)
    {
      Notify(exception, Severity.Error);
    }

    public void Notify(System.Exception exception, Severity severity)
    {
      var report = new Report(_configuration, exception, severity, BreadcrumbStore);

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

        LeaveBreadcrumb(Breadcrumb.FromReport(report));
      }
    }

    public void LeaveBreadcrumb(string message)
    {
      LeaveBreadcrumb(message, BreadcrumbType.Manual, null);
    }

    public void LeaveBreadcrumb(string message, BreadcrumbType type, IDictionary<string, string> metadata)
    {
      LeaveBreadcrumb(new Breadcrumb(message, type, metadata));
    }

    public void LeaveBreadcrumb(Breadcrumb breadcrumb)
    {
      _breadcrumbStore.Add(breadcrumb);
    }
  }
}
