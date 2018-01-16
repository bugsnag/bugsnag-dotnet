using System;
using System.Diagnostics;
using System.Web;

namespace Bugsnag.AspNet
{
  public class HttpModule : IHttpModule
  {
    private readonly Client _client;

    public HttpModule()
    {
      _client = new Client(Configuration.Settings);
    }

    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
      context.Error += OnError;
    }

    private void OnError(object sender, EventArgs e)
    {
      var application = (HttpApplication)sender;

      Notify(application.Server.GetLastError(), new HttpContextWrapper(application.Context));
    }

    private void Notify(System.Exception exception, HttpContextWrapper httpContextWrapper)
    {
      try
      {
        _client.Notify(exception, Severity.Error);
      }
      catch (System.Exception internalException)
      {
        Trace.WriteLine(internalException);
      }
    }
  }
}
