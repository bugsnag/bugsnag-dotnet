using System;

namespace Bugsnag
{
  /// <summary>
  /// Used by Bugsnag clients to send serialized error reports to an endpoint.
  /// </summary>
  public interface ITransport
  {
    void Send(Uri endpoint, byte[] report);
  }
}
