using System;
using System.Diagnostics;
using System.Web;

namespace Bugsnag.AspNet
{
  public class HttpModule : IHttpModule
  {
    private static Client _client;

    private static readonly object _lock = new object();

    private Client Client
    {
      get
      {
        lock (_lock)
        {
          if (_client == null)
          {
            _client = new Client();
          }
        }

        return _client;
      }
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

      AutoNotify(application.Server.GetLastError(), new HttpContextWrapper(application.Context));
    }

    public void AutoNotify(Exception exception, HttpContextBase httpContext)
    {
      try
      {
        Client.AutoNotify(exception, httpContext);
      }
      catch (Exception internalException)
      {
        Trace.WriteLine(internalException);
      }
    }
  }
}
