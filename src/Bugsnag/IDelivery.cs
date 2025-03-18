using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Bugsnag
{
  /// <summary>
  /// Used by Bugsnag clients to send serialized error reports to an endpoint.
  /// </summary>
  public interface IDelivery
  {
    void Send(IPayload payload);
  }

  public interface IPayload
  {
    Uri Endpoint { get; }

    KeyValuePair<string, string>[] Headers { get; }

    byte[] Serialize();
  }
}
