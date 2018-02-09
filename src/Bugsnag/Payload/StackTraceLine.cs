using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents a set of Bugsnag payload stacktrace lines that are generated from a single StackTrace provided
  /// by the runtime.
  /// </summary>
  public class StackTrace : IEnumerable<StackTraceLine>
  {
    private readonly System.Exception _originalException;

    public StackTrace(System.Exception exception)
    {
      _originalException = exception;
    }

    public IEnumerator<StackTraceLine> GetEnumerator()
    {
      if (_originalException == null)
      {
        yield break;
      }

      var needFileInfo = true;
      var stackTrace = new System.Diagnostics.StackTrace(_originalException, needFileInfo);
      var stackFrames = stackTrace.GetFrames();

      if (stackFrames == null)
      {
        yield break;
      }

      foreach (var frame in stackFrames)
      {
        var stackFrame = new StackTraceLine(frame);

        yield return stackFrame;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }

  /// <summary>
  /// Represents an individual stack trace line in the Bugsnag payload.
  /// </summary>
  public class StackTraceLine : Dictionary<string, object>
  {
    public StackTraceLine(StackFrame stackFrame)
    {
      var method = stackFrame.GetMethod();
      this.AddToPayload("file", stackFrame.GetFileName());
      this.AddToPayload("lineNumber", stackFrame.GetFileLineNumber());
      this.AddToPayload("method", new Method(method).DisplayName());
      this.AddToPayload("inProject", false);
      this.AddToPayload("code", new Code(stackFrame, 5).Display());
    }

    public string FileName
    {
      get
      {
        return (string)this["file"];
      }
      set
      {
        this["file"] = value;
      }
    }

    public string MethodName
    {
      get
      {
        return this["method"] as string;
      }
      set
      {
        this["method"] = value;
      }
    }

    public bool InProject
    {
      get
      {
        return (bool)this["inProject"];
      }
      set
      {
        this["inProject"] = value;
      }
    }
  }
}
