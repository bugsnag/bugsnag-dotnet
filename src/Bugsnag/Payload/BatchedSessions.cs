using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Bugsnag.Payload
{
  public class BatchedSessions : Dictionary<string, object>, IPayload
  {
    private readonly KeyValuePair<string, string>[] _headers;
    private readonly IConfiguration _configuration;

    public BatchedSessions(IConfiguration configuration, IEnumerable<KeyValuePair<string, long>> sessionData) : this(configuration, NotifierInfo.Instance, new App(configuration), new Device(), sessionData)
    {

    }

    public BatchedSessions(IConfiguration configuration, NotifierInfo notifier, App app, Device device, IEnumerable<KeyValuePair<string, long>> sessionData)
    {
      _configuration = configuration;
      Endpoint = configuration.SessionEndpoint;

      _headers = new KeyValuePair<string, string>[] {
        new KeyValuePair<string, string>(Payload.Headers.ApiKeyHeader, configuration.ApiKey),
        new KeyValuePair<string, string>(Payload.Headers.PayloadVersionHeader, "1.0")
      };

      this.AddToPayload("notifier", notifier);
      this.AddToPayload("device", device);
      this.AddToPayload("app", app);
      this.AddToPayload("sessionCounts", sessionData.Select(kv => new SessionCount(kv.Key, kv.Value)).ToArray());
    }

    public Uri Endpoint { get; set; }

    public KeyValuePair<string, string>[] Headers => _headers;

    public byte[] Serialize()
    {
      return Serializer.SerializeObjectToByteArray(this, _configuration.MetadataFilters);
    }
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
