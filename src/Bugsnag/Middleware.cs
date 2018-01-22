using System.Linq;

namespace Bugsnag
{
  public delegate void Middleware(IConfiguration configuration, Report report);

  static class InternalMiddleware
  {
    public static Middleware ReleaseStageFilter = (c, r) => {
      r.Deliver = r.Deliver && !c.InvalidReleaseStage();
    };

    public static Middleware RemoveFilePrefixes = (configuration, report) =>
    {
      if (configuration.FilePrefixes.Any())
      {
        foreach (var @event in report.Events)
        {
          foreach (var exception in @event.Exceptions)
          {
            foreach (var stackTraceLine in exception.StackTrace)
            {
              foreach (var filePrefix in configuration.FilePrefixes)
              {
                if (stackTraceLine.FileName.StartsWith(filePrefix, System.StringComparison.Ordinal))
                {
                  stackTraceLine.FileName = stackTraceLine.FileName.Remove(0, filePrefix.Length);
                }
              }
            }
          }
        }
      }
    };

    public static Middleware DetectInProjectNamespaces = (configuration, report) =>
    {
      if (configuration.ProjectNamespaces.Any())
      {
        foreach (var @event in report.Events)
        {
          foreach (var exception in @event.Exceptions)
          {
            foreach (var stackTraceLine in exception.StackTrace)
            {
              foreach (var @namespace in configuration.ProjectNamespaces)
              {
                stackTraceLine.InProject = stackTraceLine.MethodName.StartsWith(@namespace);
              }
            }
          }
        }
      }
    };

    public static Middleware RemoveIgnoredExceptions = (configuration, report) =>
    {
      if (configuration.IgnoreClasses.Any())
      {
        foreach (var @event in report.Events)
        {
          @event.Exceptions = @event.Exceptions.Where(e => !configuration.IgnoreClasses.Any(@class => @class == e.ErrorClass)).ToArray();
          // TODO: if we filter out all of the exceptions should we still send the report?
        }
      }
    };
  }
}
