using System;
using System.Diagnostics;
using System.Web;

namespace Bugsnag.AspNet
{
  public class HttpModule : IHttpModule
  {
    private readonly Client _client;

    private static bool initialised;

    private static readonly object _lock = new object();

    public HttpModule()
    {
      _client = new Client();
    }

    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
      lock (_lock)
      {
        if (!initialised)
        {
          context.Error += OnError;
          initialised = true;
        }
      }
    }

    private void OnError(object sender, EventArgs e)
    {
      var application = (HttpApplication)sender;

      AutoNotify(application.Server.GetLastError(), new HttpContextWrapper(application.Context));
    }

    public void AutoNotify(Exception exception, HttpContextBase httpContext)
    {
      try
      {
        _client.AutoNotify(exception, httpContext);
      }
      catch (Exception internalException)
      {
        Trace.WriteLine(internalException);
      }
    }
  }
}
