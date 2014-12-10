using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Reflection;

namespace Bugsnag.ConfigurationStorage
{
    public class BaseStorage : IConfigurationStorage
    {
        /// <summary>
        /// Gets the API key used to send notifications to a specific Bugsnag account
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets the release stage of the application
        /// </summary>
        public string ReleaseStage { get; set; }

        /// <summary>
        /// Gets or sets the endpoint that defines where to send the notifications
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets the unique identifier used to identify a user
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets the users email
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets the users human readable name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the context to apply to the subsequent notifications
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we are auto detecting method calls as 
        /// in project calls in stack traces
        /// </summary>
        public bool AutoDetectInProject { get; set; }

        /// <summary>
        /// Gets or Sets whether the notifier should register to automatically sent uncaught exceptions.
        /// </summary>
        public bool AutoNotify { get; set; }

        /// <summary>
        /// Gets or Sets the list of Release Stages to notify Bugsnag of errors in. If this is null
        /// then Bugsnag will notify on all Release Stages.
        /// </summary>
        public string[] NotifyReleaseStages { get; set; }

        /// <summary>
        /// Gets or Sets a list of prefixes to strip from filenames in the stacktrace. Helps
        /// with grouping errors from different build envs or machines by ensuring the stack
        /// trace looks similar.
        /// </summary>
        public string[] FilePrefixes { get; set; }

        /// <summary>
        /// Gets or Sets a list of namespaces to be considered in project. Helps with grouping
        /// as well as being highlighted in the Bugsnag dashboard.
        /// </summary>
        public string[] ProjectNamespaces { get; set; }

        /// <summary>
        /// Gets or Sets the list of error classes not to be notified to Bugsnag.
        /// </summary>
        public string[] IgnoreClasses { get; set; }

        /// <summary>
        /// Gets or Sets the list of keys not to be sent to Bugsnag. Add values to this list to
        /// prevent sensitive information being sent to Bugsnag.
        /// </summary>
        public string[] MetadataFilters { get; set; }

        /// <summary>
        /// Constructor for the BaseStorage class. ConfigurationStorage classes deal with storing and
        /// retrieving configuration variables for the Bugsnag notifier.
        /// </summary>
        /// <param name="apiKey">The Bugsnag apiKey</param>
        public BaseStorage(string apiKey)
        {
            ApiKey = apiKey;

            ReleaseStage = Debugger.IsAttached ? "development" : "production";
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                // Use the applicaton version defined for the Click-Once application, if it is one
                AppVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else if (Assembly.GetEntryAssembly() != null)
            {
                AppVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
            AutoDetectInProject = true;
            UserId = Environment.UserName;
            Endpoint = "notify.bugsnag.com";
        }
    }
}
