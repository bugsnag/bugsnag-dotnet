using System;
using System.Linq;
using System.Reflection;
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

        foreach (var exception in report.Event.Exceptions)
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
                  break;
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
        foreach (var exception in report.Event.Exceptions)
        {
          foreach (var stackTraceLine in exception.StackTrace)
          {
            if (!Polyfills.String.IsNullOrWhiteSpace(stackTraceLine.MethodName))
            {
              foreach (var @namespace in report.Configuration.ProjectNamespaces)
              {
                if (stackTraceLine.MethodName.StartsWith(@namespace))
                {
                  stackTraceLine.InProject = true;
                  break;
                }
              }
            }            
          }
        }
      }
    };

    /// <summary>
    /// Ignore the report if any of the exceptions in it are included in the
    /// IgnoreClasses.
    /// </summary>
    public static Middleware CheckIgnoreClasses = report =>
    {
      if (!report.Ignored && report.Configuration.IgnoreClasses != null && report.Configuration.IgnoreClasses.Any())
      {
        var containsIgnoredClass = report.Configuration.IgnoreClasses
          .Any(@class => report.Event.Exceptions
            .Any(exception => @class.IsInstanceOfType(exception.OriginalException)));

        if (containsIgnoredClass)
        {
          report.Ignore();
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
        foreach (var item in report.Configuration.GlobalMetadata)
        {
          report.Event.Metadata.Add(item.Key, item.Value);
        }
      }
    };

    /// <summary>
    /// Applies the configured metadata filters to specified sections of the report.
    /// 
    /// This is no longer used by the notifier and can be removed in the next
    /// major version bump.
    /// </summary>
    public static Middleware ApplyMetadataFilters = report =>
    {
      if (report.Configuration.MetadataFilters != null)
      {
        report.Event.App.FilterPayload(report.Configuration.MetadataFilters);
        report.Event.Device.FilterPayload(report.Configuration.MetadataFilters);
        report.Event.Metadata.FilterPayload(report.Configuration.MetadataFilters);
      }
    };

    /// <summary>
    /// Uses a request if set on the report to provide a default context.
    /// 
    /// This is no longer used by the notifier and can be removed in the next
    /// major version bump. Replaced by code in <see cref="Event.Request"/>
    /// </summary>
    public static Middleware DetermineDefaultContext = report =>
    {
      if (report.Event.Request != null && report.Event.Context == null)
      {
        if (Uri.TryCreate(report.Event.Request.Url, UriKind.Absolute, out Uri uri))
        {
          report.Event.Context = uri.AbsolutePath;
        }
        else
        {
          report.Event.Context = report.Event.Request.Url;
        }
      }
    };
  }
}
