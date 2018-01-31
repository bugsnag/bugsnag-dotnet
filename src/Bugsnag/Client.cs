using System.Collections.Generic;
using System.Diagnostics;
using Bugsnag.Payload;

namespace Bugsnag
{
  public class Client
  {
    private readonly IConfiguration _configuration;

    private readonly ITransport _transport;

    private readonly IStorage<Breadcrumb> _breadcrumbStore;

    private readonly List<Middleware> _middleware;

    private readonly object _middlewareLock = new object();

    private static Middleware[] InternalMiddleware = new Middleware[] {
      Bugsnag.InternalMiddleware.ReleaseStageFilter,
      Bugsnag.InternalMiddleware.RemoveIgnoredExceptions,
      Bugsnag.InternalMiddleware.RemoveFilePrefixes,
      Bugsnag.InternalMiddleware.DetectInProjectNamespaces,
      Bugsnag.InternalMiddleware.AttachGlobalMetadata,
    };

    public Client(IConfiguration configuration) : this(configuration, ThreadQueueTransport.Instance, new InMemoryStorage<Breadcrumb>())
    {

    }

    public Client(IConfiguration configuration, ITransport transport, IStorage<Breadcrumb> breadcrumbStore)
    {
      _configuration = configuration;
      _transport = transport;
      _breadcrumbStore = breadcrumbStore;
      _middleware = new List<Middleware>();
    }

    public IConfiguration Configuration { get { return _configuration; } }

    protected IStorage<Breadcrumb> BreadcrumbStore {  get { return _breadcrumbStore; } }

    public void BeforeNotify(Middleware middleware)
    {
      lock (_middlewareLock)
      {
        _middleware.Add(middleware);
      }
    }

    public void Notify(System.Exception exception)
    {
      var report = new Report(_configuration, exception, Payload.Severity.ForHandledException(), BreadcrumbStore);

      Notify(report);
    }

    public void Notify(System.Exception exception, Severity severity)
    {
      var report = new Report(_configuration, exception, Payload.Severity.ForUserSpecifiedSeverity(severity), BreadcrumbStore);

      Notify(report);
    }

    public void AutoNotify(System.Exception exception)
    {
      var report = new Report(_configuration, exception, Payload.Severity.ForUnhandledException(), BreadcrumbStore);

      Notify(report);
    }

    public void Notify(Report report)
    {
      foreach (var middleware in InternalMiddleware)
      {
        middleware(Configuration, report);
      }

      lock (_middlewareLock)
      {
        foreach (var middleware in _middleware)
        {
          try
          {
            middleware(Configuration, report);
          }
          catch (System.Exception exception)
          {
            Trace.WriteLine(exception);
            // we could add the exception calling the middleware to the report? Or to the metadata in the report
          }
        }
      }

      if (report.Deliver)
      {
        byte[] rawPayload = null;

        Bugsnag.InternalMiddleware.ApplyMetadataFilters(Configuration, report);

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
