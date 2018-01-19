using System.Collections;
using System.Collections.Generic;

namespace Bugsnag
{
  public class StackTrace : IEnumerable<StackTraceLine>
  {
    private readonly System.Diagnostics.StackTrace _originalStackTrace;

    public StackTrace(System.Exception exception) : this(new System.Diagnostics.StackTrace(exception, true))
    {

    }

    public StackTrace(System.Diagnostics.StackTrace stackTrace)
    {
      _originalStackTrace = stackTrace;
    }

    public IEnumerator<StackTraceLine> GetEnumerator()
    {
      var frames = _originalStackTrace.GetFrames();

      if (frames == null)
      {
        yield break;
      }

      foreach (var frame in frames)
      {
        yield return new StackTraceLine(frame);
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }

  public class StackTraceLine : Dictionary<string, object>
  {
    public StackTraceLine(System.Diagnostics.StackFrame stackFrame)
    {
      var method = stackFrame.GetMethod();

      if (method != null)
      {
        var methodName = method.FriendlyMethodName();

        var lineNumber = stackFrame.FriendlyLineNumber();

        var file = stackFrame.GetFileName();

        this.AddToPayload("file", file);
        this.AddToPayload("lineNumber", lineNumber);
        this.AddToPayload("method", methodName);
        this.AddToPayload("inProject", false);
      }

      // TODO: what should we set these to when the method is null?
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
  }
}
