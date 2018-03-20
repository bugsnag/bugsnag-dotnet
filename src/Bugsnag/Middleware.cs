using System;
using System.Linq;
using Bugsnag.Payload;

namespace Bugsnag
{
  /// <summary>
  /// Signature for Bugsnag client middleware that can be used to manipulate the
  /// error report before it is sent.
  /// </summary>
  /// <param name="report"></param>
  public delegate void Middleware(Report report);

  /// <summary>
  /// The middleware that is applied by default by the Bugsnag client.
  /// </summary>
  public static class InternalMiddleware
  {
    /// <summary>
    /// Sets the Ignore flag to true if the configuration is setup so that
    /// the report should not be sent based on the release stage information.
    /// </summary>
    public static Middleware ReleaseStageFilter = report => {
      if (report.Configuration.ReleaseStage != null
        && report.Configuration.NotifyReleaseStages != null
        && !report.Configuration.NotifyReleaseStages.Contains(report.Configuration.ReleaseStage))
      {
        report.Ignore();
      }
    };

    /// <summary>
    /// Strips any provided project roots from stack trace lines included in the report.
    /// </summary>
    public static Middleware RemoveProjectRoots = report =>
    {
      if (report.Configuration.ProjectRoots != null && report.Configuration.ProjectRoots.Any())
      {
        var projectRoots = report.Configuration.ProjectRoots.Select(prefix => {
          // if the file prefix is missing a final directory seperator then we should
          // add one first
          if (prefix[prefix.Length - 1] != System.IO.Path.DirectorySeparatorChar)
          {
            prefix = $"{prefix}{System.IO.Path.DirectorySeparatorChar}";
          }
          return prefix;
        }).ToArray();

        foreach (var @event in report.Events)
        {
          foreach (var exception in @event.Exceptions)
          {
            foreach (var stackTraceLine in exception.StackTrace)
            {
              if (!Polyfills.String.IsNullOrWhiteSpace(stackTraceLine.FileName))
              {
                foreach (var filePrefix in projectRoots)
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
      }
    };

    /// <summary>
    /// Marks stack trace lines as being 'in project' if they are from a provided namespace.
    /// </summary>
    public static Middleware DetectInProjectNamespaces = report =>
    {
      if (report.Configuration.ProjectNamespaces != null && report.Configuration.ProjectNamespaces.Any())
      {
        foreach (var @event in report.Events)
        {
          foreach (var exception in @event.Exceptions)
          {
            foreach (var stackTraceLine in exception.StackTrace)
            {
              foreach (var @namespace in report.Configuration.ProjectNamespaces)
              {
                stackTraceLine.InProject = stackTraceLine.MethodName.StartsWith(@namespace);
              }
            }
          }
        }
      }
    };

    /// <summary>
    /// Strips exceptions from the report if they include any 'ignored classes'
    /// </summary>
    public static Middleware RemoveIgnoredExceptions = report =>
    {
      if (report.Configuration.IgnoreClasses != null && report.Configuration.IgnoreClasses.Any())
      {
        foreach (var @event in report.Events)
        {
          @event.Exceptions = @event.Exceptions
            .Where(e => !report.Configuration.IgnoreClasses.Any(@class => @class == e.ErrorClass))
            .ToArray();
          // TODO: if we filter out all of the exceptions should we still send the report?
        }
      }
    };

    /// <summary>
    /// Attaches global metadata if provided by the configuration to each error report.
    /// </summary>
    public static Middleware AttachGlobalMetadata = report =>
    {
      if (report.Configuration.GlobalMetadata != null)
      {
        foreach (var @event in report.Events)
        {
          foreach (var item in report.Configuration.GlobalMetadata)
          {
            @event.Metadata.Add(item.Key, item.Value);
          }
        }
      }
    };

    public static Middleware ApplyMetadataFilters = report =>
    {
      if (report.Configuration.MetadataFilters != null)
      {
        foreach (var @event in report.Events)
        {
          @event.App.FilterPayload(report.Configuration.MetadataFilters);
          @event.Device.FilterPayload(report.Configuration.MetadataFilters);
          @event.Metadata.FilterPayload(report.Configuration.MetadataFilters);
        }
      }
    };

    public static Middleware DetermineDefaultContext = report =>
    {
      foreach (var @event in report.Events)
      {
        if (@event.Request != null)
        {
          if (Uri.TryCreate(@event.Request.Url, UriKind.Absolute, out Uri uri))
          {
            @event.Context = uri.AbsolutePath;
          }
          else
          {
            @event.Context = @event.Request.Url;
          }
        }
      }
    };
  }
}
