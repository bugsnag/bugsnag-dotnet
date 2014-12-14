namespace Bugsnag.ConfigurationStorage
{
    public interface IConfigurationStorage
    {
        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets the release stage of the application
        /// </summary>
        string ReleaseStage { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier used to identify a user
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// Gets or sets the users email
        /// </summary>
        string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the users human readable name
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Gets or sets the context to apply to the subsequent notifications
        /// </summary>
        string Context { get; set; }

        /// <summary>
        /// Gets or sets the API key used to send notifications to a specific Bugsnag account
        /// </summary>
        string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the endpoint that defines where to send the notifications
        /// </summary>
        string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we are auto detecting method calls as 
        /// in project calls in stack traces
        /// </summary>
        bool AutoDetectInProject { get; set; }

        /// <summary>
        /// Gets or Sets whether the notifier should register to automatically sent uncaught exceptions.
        /// </summary>
        bool AutoNotify { get; set; }

        /// <summary>
        /// Gets or Sets the list of Release Stages to notify Bugsnag of errors in. If this is null
        /// then Bugsnag will notify on all Release Stages.
        /// </summary>
        string[] NotifyReleaseStages { get; set; }

        /// <summary>
        /// Gets or Sets a list of prefixes to strip from filenames in the stack trace. Helps
        /// with grouping errors from different build environments or machines by ensuring the stack
        /// trace looks similar.
        /// </summary>
        string[] FilePrefixes { get; set; }

        /// <summary>
        /// Gets or Sets a list of namespaces to be considered in project. Helps with grouping
        /// as well as being highlighted in the Bugsnag dashboard.
        /// </summary>
        string[] ProjectNamespaces { get; set; }

        /// <summary>
        /// Gets or Sets the list of error classes not to be notified to Bugsnag.
        /// </summary>
        string[] IgnoreClasses { get; set; }

        /// <summary>
        /// Gets or Sets the list of keys not to be sent to Bugsnag. Add values to this list to
        /// prevent sensitive information being sent to Bugsnag.
        /// </summary>
        string[] MetadataFilters { get; set; }
    }
}
