using System;
using System.Collections.Generic;
using System.Linq;

namespace Bugsnag.Payload
{
  public class BatchedSessions : Dictionary<string, object>, ITransportablePayload
  {
    private readonly KeyValuePair<string, string>[] _headers;

    public BatchedSessions(IConfiguration configuration, IEnumerable<KeyValuePair<string, long>> sessionData) : this(configuration.ApiKey, configuration.SessionEndpoint, NotifierInfo.Instance, new App(configuration), new Device(), sessionData)
    {

    }

    public BatchedSessions(string apiKey, Uri endpoint, NotifierInfo notifier, App app, Device device, IEnumerable<KeyValuePair<string, long>> sessionData)
    {
      Endpoint = endpoint;
      _headers = new KeyValuePair<string, string>[] {
        new KeyValuePair<string, string>(Payload.Headers.ApiKeyHeader, apiKey),
        new KeyValuePair<string, string>(Payload.Headers.PayloadVersionHeader, "1.0")
      };
      this.AddToPayload("notifier", notifier);
      this.AddToPayload("device", device);
      this.AddToPayload("app", app);
      this.AddToPayload("sessionCounts", sessionData.Select(kv => new SessionCount(kv.Key, kv.Value)).ToArray());
    }

    public Uri Endpoint { get; set; }

    public KeyValuePair<string, string>[] Headers => _headers;
  }

  public class SessionCount : Dictionary<string, object>
  {
    public SessionCount(string startedAt, long sessionStarted)
    {
      this.AddToPayload("startedAt", startedAt);
      this.AddToPayload("sessionsStarted", sessionStarted);
    }
  }
}
