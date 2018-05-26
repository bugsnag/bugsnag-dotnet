using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Bugsnag.Tests
{
  public class TestServer
  {
    private readonly IWebHost _webHost;
    private readonly RequestCollection<string> _requests;
    private readonly int _port;

    public TestServer()
    {
      _port = PortAllocations.Instance.NextFreePort();
      _requests = new RequestCollection<string>();
      _webHost = new WebHostBuilder()
        .ConfigureServices(services => services.AddSingleton(typeof(RequestCollection<string>), _requests))
        .UseStartup<Startup>()
        .UseKestrel(options => {
          options.Listen(IPAddress.Loopback, _port);
        })
        .Build();
    }

    public Uri Endpoint { get { return new Uri($"http://localhost:{_port}"); } }

    public void Start()
    {
      _webHost.Start();
    }

    public async Task<IEnumerable<string>> Requests(int numberOfRequests)
    {
      var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
      return await Requests(numberOfRequests, cts.Token);
    }

    public async Task<IEnumerable<string>> Requests(int numberOfRequests, CancellationToken token)
    {
      var items = await _requests.Items(numberOfRequests, token);
      await _webHost.StopAsync();
      return items;
    }

    class PortAllocations
    {
      private static readonly Lazy<PortAllocations> instance = new Lazy<PortAllocations>(() => new PortAllocations());

      public static PortAllocations Instance { get { return instance.Value; } }

      private int _nextFreePort;

      private PortAllocations()
      {
        _nextFreePort = 4999;
      }

      public int NextFreePort()
      {
        return Interlocked.Increment(ref _nextFreePort);
      }
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
      private readonly List<T> _requests;
      private readonly object _requestsLock;
      private readonly TaskCompletionSource<List<T>> _taskCompletionSource;

      public RequestCollection()
      {
        _taskCompletionSource = new TaskCompletionSource<List<T>>();
        _requests = new List<T>();
        _requestsLock= new object();
      }

      public void Add(T item)
      {
        lock (_requestsLock)
        {
          _requests.Add(item);
        }
      }

      public Task<List<T>> Items(int numberOfRequests, CancellationToken token)
      {
        return Task.Factory.StartNew(async () =>
        {
          while (true)
          {
            lock (_requestsLock)
            {
              if (_requests.Count >= numberOfRequests)
              {
                return new List<T>(_requests);
              }
            }
            token.ThrowIfCancellationRequested();
            await Task.Delay(TimeSpan.FromSeconds(1));
          }
        }, token).Unwrap();
      }
    }
  }
}
