using Bugsnag.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bugsnag.Tests
{
  public class DefaultDeliveryTests
  {
    private const string API_KEY = "a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6";

    [Fact]
    public async Task Send_ReportPayload_SendsCorrectly()
    {
      var server = new TestServer();
      server.Start();

      var configuration = new Configuration(API_KEY);
      configuration.Endpoint = server.Endpoint;

      var report = new Report(configuration, new System.Exception("Test exception"), HandledState.ForHandledException(), new List<Breadcrumb>(), new Session());

      DefaultDelivery.Instance.Send(report);

      var requests = await server.Requests(1);
      var request = requests.First();

      Assert.Contains(request.Headers, h => h.Key == "Bugsnag-Api-Key" && h.Value == API_KEY);
      Assert.Contains(request.Headers, h => h.Key == "Bugsnag-Payload-Version" && h.Value == "4");
      Assert.Contains(request.Headers, h => h.Key == "Bugsnag-Sent-At");

      Assert.Contains($"\"apiKey\":\"{API_KEY}\"", request.Body);
      Assert.Contains("\"events\":[", request.Body);
      Assert.Contains("\"exceptions\":[", request.Body);
      Assert.Contains("\"message\":\"Test exception\"", request.Body);
    }

    [Fact]
    public async Task Send_BatchedSessionsPayload_SendsCorrectly()
    {
      var server = new TestServer();
      server.Start();

      var configuration = new Configuration(API_KEY);
      configuration.SessionEndpoint = server.Endpoint;

      var session = new Session();
      var sessionData = new List<KeyValuePair<string, long>> { new KeyValuePair<string, long>(session.SessionKey, 1) };
      var batchedSessions = new BatchedSessions(configuration, sessionData);

      DefaultDelivery.Instance.Send(batchedSessions);

      var requests = await server.Requests(1);
      var request = requests.First();

      Assert.Contains(request.Headers, h => h.Key == "Bugsnag-Api-Key" && h.Value == API_KEY);
      Assert.Contains(request.Headers, h => h.Key == "Bugsnag-Payload-Version" && h.Value == "1.0");
      Assert.Contains(request.Headers, h => h.Key == "Bugsnag-Sent-At");

      Assert.Contains("\"sessionCounts\":[", request.Body);
      Assert.Contains($"\"startedAt\":\"{session.SessionKey}\"", request.Body);
      Assert.Contains("\"sessionsStarted\":1", request.Body);
    }

    [Fact]
    public async Task Send_EnqueuesAndSendsPayloads()
    {
      var numberOfRequests = 500;

      var server = new TestServer();

      server.Start();

      for (int i = 0; i < numberOfRequests; i++)
      {
        var payload = new SamplePayload(i, server.Endpoint);
        DefaultDelivery.Instance.Send(payload);
      }

      var requests = await server.Requests(numberOfRequests);

      Assert.Equal(numberOfRequests, requests.Count());
    }

    private class SamplePayload : IPayload
    {
      public SamplePayload(int count, Uri endpoint)
      {
        Count = count;
        Endpoint = endpoint;
      }

      public Uri Endpoint { get; set; }

      public IWebProxy Proxy { get; set; }

      public KeyValuePair<string, string>[] Headers => new KeyValuePair<string, string>[] { };

      public int Count { get; set; }

      public byte[] Serialize()
      {
        return Encoding.UTF8.GetBytes($"payload {Count}");
      }
    }
  }
}
