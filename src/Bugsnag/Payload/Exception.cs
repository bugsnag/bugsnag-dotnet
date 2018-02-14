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
      // ReflectionTypeLoadException is special because the details are in
      // the LoaderExceptions property
      if (ex is ReflectionTypeLoadException typeLoadException)
      {
        var typeLoadExceptions = new List<System.Exception>();
        foreach (var loadException in typeLoadException.LoaderExceptions)
        {
          typeLoadExceptions.AddRange(FlattenAndReverseExceptionTree(loadException));
        }

        typeLoadExceptions.Add(ex);
        return typeLoadExceptions;
      }

      var list = new List<System.Exception>();
      while (ex != null)
      {
        list.Add(ex);
        ex = ex.InnerException;
      }
      return list;
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
