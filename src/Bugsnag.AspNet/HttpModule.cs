using System;
using System.Diagnostics;
using System.Web;

namespace Bugsnag.AspNet
{
  public class HttpModule : IHttpModule
  {
    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
      context.Error += OnError;
      context.BeginRequest += OnBeginRequest;
    }

    private void OnBeginRequest(object sender, EventArgs e)
    {
      Bugsnag.Singleton.Client.SessionTracking.CreateSession();
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
        Bugsnag.Singleton.Client.AutoNotify(exception, httpContext);
      }
      catch (Exception internalException)
      {
        Trace.WriteLine(internalException);
      }
    }
  }
}
