using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Bugsnag.Tests
{
  public class TestServer
  {
    private readonly IWebHost _webHost;
    private readonly RequestCollection<string> _requests;

    public TestServer(int port, int expectedNumberOfRequests)
    {
      _requests = new RequestCollection<string>(expectedNumberOfRequests);
      _webHost = new WebHostBuilder()
        .ConfigureServices(services => services.AddSingleton(typeof(RequestCollection<string>), _requests))
        .UseStartup<Startup>()
        .UseKestrel(options => {
          options.Listen(IPAddress.Loopback, port);
        })
        .Build();
    }

    public void Start()
    {
      _webHost.Start();
    }

    public async Task<IEnumerable<string>> Requests()
    {
      var items = await _requests.Items();
      await _webHost.StopAsync();
      return items;
    }

    class Startup
    {
      private readonly RequestCollection<string> _requests;

      public Startup(RequestCollection<string> requests)
      {
        _requests = requests;
      }

      public void Configure(IApplicationBuilder app)
      {
        app.Run(async context => {
          var stream = context.Request.Body;
          using (var reader = new StreamReader(stream))
          {
            var request = await reader.ReadToEndAsync();
            _requests.Add(request);
          }
          await context.Response.WriteAsync("OK");
        });
      }
    }

    class RequestCollection<T>
    {
      private readonly int _expectedNumberOfRequests;
      private readonly List<T> _requests;
      private readonly object _requestsLock;
      private readonly TaskCompletionSource<List<T>> _taskCompletionSource;

      public RequestCollection(int expectedNumberOfRequests)
      {
        _taskCompletionSource = new TaskCompletionSource<List<T>>();
        _expectedNumberOfRequests = expectedNumberOfRequests;
        _requests = new List<T>(expectedNumberOfRequests);
        _requestsLock= new object();
      }

      public void Add(T item)
      {
        lock (_requestsLock)
        {
          _requests.Add(item);

          if (_requests.Count >= _expectedNumberOfRequests)
          {
            _taskCompletionSource.SetResult(_requests);
          }
        }
      }

      public Task<List<T>> Items()
      {
        return _taskCompletionSource.Task;
      }
    }
  }
}
