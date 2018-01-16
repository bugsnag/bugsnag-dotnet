using System.Linq;
using System.Collections.Generic;

namespace Bugsnag
{
  public class ReportFactory
  {
    private readonly string _apiKey;
    private readonly App _app;

    public ReportFactory(IConfiguration configuration)
    {
      _app = new App(configuration.AppVersion, configuration.ReleaseStage, configuration.AppType);
      _apiKey = configuration.ApiKey;
    }

    public Report Generate(System.Exception exception, Severity severity)
    {
      var exceptions = GenerateExceptions(exception).ToArray();
      var @event = new Event(exceptions, severity, _app);
      var report = new Report(_apiKey, @event) { Exception = exception };

      return report;
    }

    public IEnumerable<Exception> GenerateExceptions(System.Exception exception)
    {
      yield return GenerateException(exception);

      if (exception.InnerException != null)
      {
        yield return GenerateException(exception.InnerException);
      }
    }

    public Exception GenerateException(System.Exception exception)
    {
      var exceptionType = exception.GetType();

      var className = exceptionType.FriendlyClassName();

      return new Exception(className, exception.Message, GenerateStackTrace(exception).ToArray());
    }

    public IEnumerable<StackTraceLine> GenerateStackTrace(System.Exception exception)
    {
      return GenerateStackTrace(new System.Diagnostics.StackTrace(exception, true));
    }

    public IEnumerable<StackTraceLine> GenerateStackTrace(System.Diagnostics.StackTrace stackTrace)
    {
      var frames = stackTrace.GetFrames();

      if (frames == null)
      {
        yield break;
      }

      foreach (var frame in frames)
      {
        var stackTraceLine = GenerateStackTraceLine(frame);

        if (stackTraceLine != null)
        {
          yield return stackTraceLine;
        }
      }
    }

    public StackTraceLine GenerateStackTraceLine(System.Diagnostics.StackFrame frame)
    {
      var method = frame.GetMethod();

      if (method != null)
      {
        var methodName = method.FriendlyMethodName();

        var lineNumber = frame.FriendlyLineNumber();

        var file = frame.GetFileName();

        return new StackTraceLine(file, lineNumber, methodName, false);
      }

      return null;
    }
  }
}
