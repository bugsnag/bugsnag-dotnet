using System;
using System.Collections.Generic;
using System.Linq;
using Bugsnag.ConfigurationStorage;

namespace Bugsnag
{
    /// <summary>
    /// Defines the configuration for a sending notifications to Bugsnag
    /// </summary>
    public class Configuration : IConfigurationStorage
    {
        #region IConfigurationStorage
        /// <summary>
        /// Gets the API key used to send notifications to a specific Bugsnag account
        /// </summary>
        public string ApiKey
        {
            get { return Storage.ApiKey; }
            set { Storage.ApiKey = value; }
        }

        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        public string AppVersion
        {
            get { return Storage.AppVersion; }
            set { Storage.AppVersion = value; }
        }

        /// <summary>
        /// Gets or sets the release stage of the application
        /// </summary>
        public string ReleaseStage
        {
            get { return Storage.ReleaseStage; }
            set { Storage.ReleaseStage = value; }
        }

        /// <summary>
        /// Gets or sets the endpoint that defines where to send the notifications
        /// </summary>
        public string Endpoint
        {
            get { return Storage.Endpoint; }
            set { Storage.Endpoint = value; }
        }

        /// <summary>
        /// Gets the unique identifier used to identify a user
        /// </summary>
        public string UserId
        {
            get { return Storage.UserId; }
            set { Storage.UserId = value; }
        }

        /// <summary>
        /// Gets the users email
        /// </summary>
        public string UserEmail
        {
            get { return Storage.UserEmail; }
            set { Storage.UserEmail = value; }
        }

        /// <summary>
        /// Gets the users human readable name
        /// </summary>
        public string UserName
        {
            get { return Storage.UserName; }
            set { Storage.UserName = value; }
        }

        /// <summary>
        /// Gets or sets the context to apply to the subsequent notifications
        /// </summary>
        public string Context
        {
            get { return Storage.Context; }
            set { Storage.Context = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we are auto detecting method calls as 
        /// in project calls in stack traces
        /// </summary>
        public bool AutoDetectInProject
        {
            get { return Storage.AutoDetectInProject; }
            set { Storage.AutoDetectInProject = value; }
        }

        /// <summary>
        /// Gets or Sets whether the notifier should register to automatically sent uncaught exceptions.
        /// </summary>
        public bool AutoNotify
        {
            get { return Storage.AutoNotify; }
            set
            {
                Storage.AutoNotify = value;
                //TODO Should emit event to client to properly support this after initialisation
            }
        }

        /// <summary>
        /// Gets or Sets the list of Release Stages to notify Bugsnag of errors in. If this is null
        /// then Bugsnag will notify on all Release Stages.
        /// </summary>
        public string[] NotifyReleaseStages
        {
            get { return Storage.NotifyReleaseStages; }
            set { Storage.NotifyReleaseStages = value; }
        }

        /// <summary>
        /// Gets or Sets a list of prefixes to strip from filenames in the stacktrace. Helps
        /// with grouping errors from different build envs or machines by ensuring the stack
        /// trace looks similar.
        /// </summary>
        public string[] FilePrefixes
        {
            get { return Storage.FilePrefixes; }
            set { Storage.FilePrefixes = value; }
        }

        /// <summary>
        /// Gets or Sets a list of namespaces to be considered in project. Helps with grouping
        /// as well as being highlighted in the Bugsnag dashboard.
        /// </summary>
        public string[] ProjectNamespaces
        {
            get { return Storage.ProjectNamespaces; }
            set { Storage.ProjectNamespaces = value; }
        }

        /// <summary>
        /// Gets or Sets the list of error classes not to be notified to Bugsnag.
        /// </summary>
        public string[] IgnoreClasses
        {
            get { return Storage.IgnoreClasses; }
            set { Storage.IgnoreClasses = value; }
        }

        /// <summary>
        /// Gets or Sets the list of keys not to be sent to Bugsnag. Add values to this list to
        /// prevent sensitive information being sent to Bugsnag.
        /// </summary>
        public string[] MetadataFilters
        {
            get { return Storage.MetadataFilters; }
            set { Storage.MetadataFilters = value; }
        }

        public bool StoreOfflineErrors
        {
            get { return Storage.StoreOfflineErrors; }
            set { Storage.StoreOfflineErrors = value; }
        }
        #endregion

        /// <summary>
        /// Gets the metadata to send with every error report
        /// </summary>
        public Metadata Metadata { get; protected set; }

        /// <summary>
        /// Gets the endpoint URL that notifications will be send to
        /// </summary>
        public Uri EndpointUrl
        {
            get { return new Uri(Endpoint); }
        }

        /// <summary>
        /// Gets or sets a list of custom functions to run just before a notification is sent, 
        /// the functions operate on an Event and returns a boolean indicating if the notification 
        /// should continue to be reported
        /// </summary>
        protected List<Func<Event, bool>> BeforeNotifyCallbacks { get; set; }

        /// <summary>
        /// Gets or sets a list of internal functions to run just before a notification is sent, 
        /// the functions operate on an Event. 
        /// </summary>
        protected List<Action<Event>> InternalBeforeNotifyCallbacks { get; set; }

        /// <summary>
        /// Gets or Sets the Storage backend for the Configuration
        /// </summary>
        protected IConfigurationStorage Storage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class. Produces a default configuration with 
        /// default configuration settings
        /// </summary>
        /// <param name="apiKey">The API key linked to a Bugsnag account</param>
        public Configuration(string apiKey)
            : this(new BaseStorage(apiKey))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class. Uses the passed
        /// configuration storage to populate its settings.
        /// </summary>
        /// <param name="storage">The storage to use for settings.</param>
        public Configuration(IConfigurationStorage storage)
        {
            Storage = storage;
            Metadata = new Metadata();
            BeforeNotifyCallbacks = new List<Func<Event, bool>>();
            InternalBeforeNotifyCallbacks = new List<Action<Event>>();
        }

        /// <summary>
        /// Sets the information about the current user of the system
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="userEmail">The email of the user</param>
        /// <param name="userName">The name of the user</param>
        public void SetUser(string userId, string userEmail, string userName)
        {
            UserId = userId;
            UserEmail = userEmail;
            UserName = userName;
        }

        /// <summary>
        /// Adds a BeforeNotify callback to be called before an error is sent to Bugsnag
        /// </summary>
        /// <param name="callback">The callback to be called before notifying Bugsnag</param>
        public void BeforeNotify(Func<Event, bool> callback)
        {
            BeforeNotifyCallbacks.Add(callback);
        }

        /// <summary>
        /// Add an internal beforeNotify callback. These are run before any user defined
        /// callbacks so the user can always override default derived values.
        /// </summary>
        /// <param name="callback">The callback to be called before notifying Bugsnag</param>
        internal void BeforeNotify(Action<Event> callback)
        {
            InternalBeforeNotifyCallbacks.Add(callback);
        }

        /// <summary>
        /// Detects if the current release stage is a stage that should be notified on
        /// </summary>
        /// <returns>True if we should notify, otherwise false</returns>
        internal bool IsNotifyReleaseStage()
        {
            // Notify if no restrictions have been set or the release stage hasn't been set
            if (NotifyReleaseStages == null || ReleaseStage == null)
                return true;

            return NotifyReleaseStages.Any(x => x == ReleaseStage);
        }

        /// <summary>
        /// Removes all file prefixes from a filename
        /// </summary>
        /// <param name="fileName">The filename to modify</param>
        /// <returns>The filename with the prefixes removed</returns>
        internal string RemoveFileNamePrefix(string fileName)
        {
            var result = fileName;
            if (!string.IsNullOrEmpty(result))
            {
                foreach (string prefix in FilePrefixes)
                {
                    result = result.Replace(prefix, string.Empty);
                }
            }
            return result;
        }

        /// <summary>
        /// Normalize the directory separators in the filename
        /// </summary>
        /// <param name="filename">The filename to modify</param>
        /// <returns>The filename with normalized directory separators</returns>
        internal string NormalizePath(string filename)
        {
            return filename != null ? filename.Replace('/', '\\') : null;
        }

        /// <summary>
        /// Indicates if a method name belongs to In Project namespaces
        /// </summary>
        /// <param name="fullMethodName">The fully qualified method name</param>
        /// <returns>True if it belongs to one of the project namespaces, otherwise false</returns>
        internal bool IsInProjectNamespace(string fullMethodName)
        {
            if (string.IsNullOrEmpty(fullMethodName))
                return false;

            return ProjectNamespaces.Any(x => fullMethodName.StartsWith(x));
        }

        /// <summary>
        /// Indicates if an exception class should be ignored based on the previously set ignore
        /// class list
        /// </summary>
        /// <param name="className">The exception class name to check</param>
        /// <returns>True if the class should be ignored, otherwise false</returns>
        internal bool IsClassToIgnore(string className)
        {
            return IgnoreClasses.Contains(className);
        }

        /// <summary>
        /// Indicates if an entry key is a entry that should be filtered
        /// </summary>
        /// <param name="entry">The entry to check</param>
        /// <returns>True if the entry should be filtered, otherwise False</returns>
        internal bool IsEntryFiltered(string entry)
        {
            return MetadataFilters.Contains(entry);
        }

        /// <summary>
        /// Runs the internal before notify callbacks which are configured by the notifier itself
        /// to add some default values.
        /// </summary>
        /// <param name="errorEvent">The event that will be sent to bugsnag</param>
        internal void RunInternalBeforeNotifyCallbacks(Event errorEvent)
        {
            // Do nothing if the before notify action indicates we should ignore the error event
            foreach (Action<Event> callback in InternalBeforeNotifyCallbacks)
            {
                try
                {
                    callback(errorEvent);
                }
                catch (Exception exp)
                {
                    // If the callback exceptions, we will try to send the notification anyway, to give the
                    // best possible chance of reporting the error
                    Logger.Warning("[Before Notify] Exception : " + exp.ToString());
                }
            }
        }

        /// <summary>
        /// Copy into the Event the information from the config.
        /// </summary>
        /// <param name="errorEvent">The event to add the info to</param>
        internal void AddConfigToEvent(Event errorEvent)
        {
            if (!String.IsNullOrEmpty(Context)) errorEvent.Context = Context;
            if (!String.IsNullOrEmpty(UserId)) errorEvent.UserId = UserId;
            if (!String.IsNullOrEmpty(UserName)) errorEvent.UserName = UserName;
            if (!String.IsNullOrEmpty(UserEmail)) errorEvent.UserEmail = UserEmail;
        }

        /// <summary>
        /// Runs all the before notify callbacks with the supplied error.
        /// </summary>
        /// <param name="errorEvent">The error that will be sent to Bugsnag</param>
        /// <returns>True if all callbacks returned true, false otherwise</returns>
        internal bool RunBeforeNotifyCallbacks(Event errorEvent)
        {
            // Do nothing if the before notify action indicates we should ignore the error event
            foreach (Func<Event, bool> callback in BeforeNotifyCallbacks)
            {
                try
                {
                    if (!callback(errorEvent))
                        return false;
                }
                catch (Exception exp)
                {
                    // If the callback exceptions, we will try to send the notification anyway, to give the
                    // best possible chance of reporting the error
                    Logger.Warning("[Before Notify] Exception : " + exp.ToString());
                }
            }
            var state = errorEvent.HandledState;
            if (state.CurrentSeverity != state.OriginalSeverity)
                state.SeverityReason = SeverityReason.CallbackSpecified;
            return true;
        }
    }
}
