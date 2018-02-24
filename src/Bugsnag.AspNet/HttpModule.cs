using System;
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
      var application = (HttpApplication)sender;

      var client = new Bugsnag.Client(ConfigurationSection.Configuration.Settings);

      application.Context.Items[Client.HttpContextItemsKey] = client;

      client.SessionTracking.CreateSession();
    }

    private void OnError(object sender, EventArgs e)
    {
      var application = (HttpApplication)sender;

      var exception = application.Server.GetLastError();

      var httpContext = new HttpContextWrapper(application.Context);

      if (application.Context.Items[Client.HttpContextItemsKey] is IClient client)
      {
        client.AutoNotify(exception, httpContext);
      }
    }
  }
}
