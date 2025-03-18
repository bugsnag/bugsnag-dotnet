using Bugsnag.Payload;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bugsnag
{
  public class DefaultDelivery : IDelivery
  {
    private static DefaultDelivery instance = null;
    private static readonly object instanceLock = new object();

    private HttpClient _httpClient;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _processingTask;
    private readonly BlockingCollection<IPayload> _queue;

    private DefaultDelivery()
    {
      _httpClient = new HttpClient();
      _queue = new BlockingCollection<IPayload>(new ConcurrentQueue<IPayload>());
      _cancellationTokenSource = new CancellationTokenSource();
      _processingTask = Task.Run(() => ProcessQueueAsync(_cancellationTokenSource.Token));
    }

    public static DefaultDelivery Instance
    {
      get
      {
        lock (instanceLock)
        {
          if (instance == null)
          {
            instance = new DefaultDelivery();
          }

          return instance;
        }
      }
    }

    public void Configure(IConfiguration configuration)
    {
      if (configuration.Proxy != null)
      {
        _httpClient.Dispose();
        _httpClient = new HttpClient(new HttpClientHandler { Proxy = configuration.Proxy });
      }
    }

    public void Send(IPayload payload)
    {
      _queue.Add(payload);
    }

    internal void Stop()
    {
      Task.WaitAll(new[] { _processingTask }, TimeSpan.FromSeconds(5));
      _cancellationTokenSource.Cancel();
      _httpClient.Dispose();
      _cancellationTokenSource.Dispose();
      _queue.Dispose();
    }

    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        IPayload payload = null;

        try
        {
          // Take will block until an item is available or cancellation is requested
          payload = _queue.Take(cancellationToken);
        }
        catch (OperationCanceledException)
        {
          // Exit gracefully when cancellation is requested
          break;
        }

        if (payload != null)
        {
          await SendPayloadAsync(payload);
        }
      }
    }

    private async Task SendPayloadAsync(IPayload payload)
    {
      try
      {
        byte[] serializedPayload = payload.Serialize();
        if (serializedPayload == null)
          return;

        using (var request = new HttpRequestMessage(HttpMethod.Post, payload.Endpoint))
        {
          request.Content = new ByteArrayContent(serializedPayload);
          request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

          // Add headers from payload
          if (payload.Headers != null)
          {
            foreach (var header in payload.Headers)
            {
              request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
          }

          // Add the Bugsnag-Sent-At header
          request.Headers.Add("Bugsnag-Sent-At", DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture));

          // Send the request and log any failures
          var response = await _httpClient.SendAsync(request);

          if (!response.IsSuccessStatusCode) {
            Trace.WriteLine($"Failed to send payload to Bugsnag - received status code {response.StatusCode}");
          }
        }
      }
      catch (System.Exception ex)
      {
        Trace.WriteLine($"Error sending payload to Bugsnag: {ex}");
      }
    }
  }
}
