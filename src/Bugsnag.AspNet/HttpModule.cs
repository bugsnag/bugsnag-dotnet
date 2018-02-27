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

      if (client.Configuration.AutoCaptureSessions)
      {
        client.SessionTracking.CreateSession();
      }
    }

    private void OnError(object sender, EventArgs e)
    {
      var application = (HttpApplication)sender;

      if (application.Context.Items[Client.HttpContextItemsKey] is IClient client)
      {
        var exception = application.Server.GetLastError();

        client.AutoNotify(exception, application.Context);
      }
    }
  }
}
