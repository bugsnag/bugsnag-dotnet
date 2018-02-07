using System;
using System.Collections.Generic;

namespace Bugsnag
{
  public interface IConfiguration
  {
    /// <summary>
    /// The API Key that the Bugsnag client will submit error reports to the Bugsnag
    /// API.
    /// </summary>
    string ApiKey { get; }

    /// <summary>
    /// The endpoint that Bugsnag will submit error reports to, this should default to
    /// https://notify.bugsnag.com
    /// </summary>
    Uri Endpoint { get; }

    /// <summary>
    /// The release stage that the application is currently running in eg. development, staging, production etc.
    /// </summary>
    string ReleaseStage { get; }

    /// <summary>
    /// Filter error reports from being sent to Bugsnag unless the ReleaseStage property is included in this list
    /// </summary>
    string[] NotifyReleaseStages { get; }

    /// <summary>
    /// The version of the application that Bugsnag will attach to error reports
    /// </summary>
    string AppVersion { get; }

    /// <summary>
    /// The type of application that is being run that Bugsnag will attach to error reports.
    /// </summary>
    string AppType { get; }

    /// <summary>
    /// These will be used to strip the beginning of the file name from stack trace lines in order to produce uniform
    /// stack trace file names to aid in grouping.
    /// </summary>
    string[] FilePrefixes { get; }

    /// <summary>
    /// These will be used to mark stack trace lines as being 'In Project' if the line occurs in one of the provided
    /// namespaces. This will aid in grouping.
    /// </summary>
    string[] ProjectNamespaces { get; }

    /// <summary>
    /// These will be used to filter exceptions from being sent to Bugsnag based on the Error Class of the exception.
    /// eg. "System.FileNotFoundException"
    /// </summary>
    string[] IgnoreClasses { get; }

    /// <summary>
    /// This can be used to include these values as metadata in all error reports submitted to Bugsnag.
    /// </summary>
    KeyValuePair<string, string>[] GlobalMetadata { get; }

    /// <summary>
    /// Used to filter these keys from data being sent to Bugsnag.
    /// </summary>
    string[] MetadataFilters { get; }

    /// <summary>
    /// Should the client send session tracking information.
    /// </summary>
    bool TrackSessions { get; }

    /// <summary>
    /// The endpoint that the Bugsnag client will submit session data to.
    /// </summary>
    Uri SessionEndpoint { get; }

    /// <summary>
    /// Used to determine how often to flush session data to Bugsnag.
    /// </summary>
    TimeSpan SessionTrackingInterval { get; }
  }
}
