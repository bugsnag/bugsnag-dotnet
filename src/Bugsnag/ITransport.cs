using System;
using System.Collections;
using System.Collections.Generic;

namespace Bugsnag
{
  /// <summary>
  /// Used by Bugsnag clients to send serialized error reports to an endpoint.
  /// </summary>
  public interface ITransport
  {
    void Send(ITransportablePayload payload);
  }

  public interface ITransportablePayload : IDictionary
  {
    Uri Endpoint { get; }

    KeyValuePair<string, string>[] Headers { get; }
  }
}
