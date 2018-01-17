using System;

namespace Bugsnag
{
  public interface ITransport
  {
    void Send(Uri endpoint, byte[] report);
  }
}
