using System;
using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bugsnag.AspNet.WebApi
{
  public class DelegatingHandler : System.Net.Http.DelegatingHandler
  {
    private readonly IConfiguration _configuration;

    public DelegatingHandler(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      IClient client = null;

      // if the request has this property then we are not being self hosted
      // therefore we need to try and find the client that was setup via the
      // http module
      if (request.Properties.ContainsKey("MS_HttpContext"))
      {
        var possibleHttpContext = request.Properties["MS_HttpContext"];

        if (possibleHttpContext != null)
        {
          object potentialItems = null;

          try
          {
            potentialItems = HttpContextGetter(possibleHttpContext)(possibleHttpContext);
          }
          catch (Exception ex)
          {
            Trace.WriteLine(ex);
          }

          if (potentialItems is IDictionary dictionary)
          {
            if (dictionary.Contains(Client.HttpContextItemsKey))
            {
              client = dictionary[Client.HttpContextItemsKey] as IClient;
            }
          }
        }
      }

      if (client == null)
      {
        client = new Bugsnag.Client(_configuration);
      }

      if (client.Configuration.AutoCaptureSessions)
      {
        client.SessionTracking.CreateSession();
      }

      request.UseBugsnagClient(client);

      return base.SendAsync(request, cancellationToken);
    }

    private static Func<object, object> _cached;
    private static readonly object _lock = new object();

    /// <summary>
    /// Build a property getter delegate for faster access to this property
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    private static Func<object, object> HttpContextGetter(object o)
    {
      lock (_lock)
      {
        if (_cached == null)
        {
          var type = o.GetType();
          var property = type.GetProperty("Items", typeof(IDictionary));
          if (property == null) return a => a; // this is not an HttpContext
          var instance = Expression.Parameter(typeof(object), "instance");
          var cast = Expression.TypeAs(instance, property.DeclaringType);
          _cached = Expression.Lambda<Func<object, object>>(Expression.TypeAs(Expression.Call(cast, property.GetGetMethod()), typeof(object)), instance).Compile();
        }
      }

      return _cached;
    }
  }
}
