using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents a set of Bugsnag payload exceptions that are generated from a single exception by resolving
  /// the inner exceptions present.
  /// </summary>
  public class Exceptions : IEnumerable<Exception>
  {
    private readonly System.Exception _originalException;
    private readonly int _sourceCodeLineCount;
    private readonly IEnumerable<Exception> _exceptions;

    public Exceptions(System.Exception exception, int sourceCodeLineCount)
    {
      _originalException = exception;
      _sourceCodeLineCount = sourceCodeLineCount;
      _exceptions = FlattenAndReverseExceptionTree(exception).Select(e => new Exception(e));
    }

    public IEnumerator<Exception> GetEnumerator()
    {
      return _exceptions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private static IEnumerable<System.Exception> FlattenAndReverseExceptionTree(System.Exception ex)
    {
      if (ex == null) yield break;

      switch (ex)
      {
        case ReflectionTypeLoadException typeLoadException:
          foreach (var exception in typeLoadException.LoaderExceptions)
          {
            foreach (var item in FlattenAndReverseExceptionTree(exception))
            {
              yield return item;
            }
          }
          break;
#if !NET35
        case System.AggregateException aggregateException:
          foreach (var exception in aggregateException.InnerExceptions)
          {
            foreach (var item in FlattenAndReverseExceptionTree(exception))
            {
              yield return item;
            }
          }
          break;
#endif
        default:
          foreach (var item in FlattenAndReverseExceptionTree(ex.InnerException))
          {
            yield return item;
          }
          break;
      }

      yield return ex;
    }
  }

  /// <summary>
  /// Represents an individual exception in the Bugsnag payload.
  /// </summary>
  public class Exception : Dictionary<string, object>
  {
    public Exception(System.Exception exception)
    {
      this.AddToPayload("errorClass", TypeNameHelper.GetTypeDisplayName(exception.GetType()));
      this.AddToPayload("message", exception.Message);
      this.AddToPayload("stacktrace", new StackTrace(exception).ToArray());
    }

    public IEnumerable<StackTraceLine> StackTrace { get { return this.Get("stacktrace") as IEnumerable<StackTraceLine>; } }

    public string ErrorClass { get { return this.Get("errorClass") as string; } }
  }
}
