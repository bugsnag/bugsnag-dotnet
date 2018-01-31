using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents a set of Bugsnag payload exceptions that are generated from a single exception by resolving
  /// the inner exceptions present.
  /// </summary>
  public class Exceptions : IEnumerable<Exception>
  {
    private readonly System.Exception _originalException;

    public Exceptions(System.Exception exception)
    {
      _originalException = exception;
    }

    public IEnumerator<Exception> GetEnumerator()
    {
      yield return new Exception(_originalException);

      if (_originalException.InnerException != null)
      {
        yield return new Exception(_originalException.InnerException);
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }

  /// <summary>
  /// Represents an individual exception in the Bugsnag payload.
  /// </summary>
  public class Exception : Dictionary<string, object>
  {
    public Exception(System.Exception exception)
    {
      this.AddToPayload("errorClass", exception.GetType().FriendlyClassName());
      this.AddToPayload("message", exception.Message);
      this.AddToPayload("stacktrace", new StackTrace(exception).ToArray());
    }

    public IEnumerable<StackTraceLine> StackTrace { get { return this["stacktrace"] as IEnumerable<StackTraceLine>; } }

    public string ErrorClass { get { return this["errorClass"] as string; } }
  }
}
