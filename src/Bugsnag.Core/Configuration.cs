using System;
using System.Collections.Generic;
using System.Linq;

namespace Bugsnag.Core
{
    /// <summary>
    /// Defines the configuration for a sending notifications to Bugsnag
    /// </summary>
    public class Configuration : IConfiguration
    {
        /// <summary>
        /// Gets the API key used to send notifications to a specific Bugsnag account
        /// </summary>
        public string ApiKey { get; private set; }

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
        /// Gets or sets a value indicating whether the notifications should be sent using SSL
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// Gets the unique identifier used to identify a user
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the users email
        /// </summary>
        public string UserEmail { get; private set; }

        /// <summary>
        /// Gets the users human readable name
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the current logged in user name
        /// </summary>
        public string LoggedOnUser { get; private set; }

        /// <summary>
        /// Gets or sets the context to apply to the subsequent notifications
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Gets the metadata to send with every error report
        /// </summary>
        public Metadata Metadata { get; private set; }

        /// <summary>
        /// Gets the endpoint URL that notifications will be send to
        /// </summary>
        public Uri EndpointUrl
        {
            get { return new Uri((UseSsl ? @"https://" : "http://") + Endpoint); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we are auto detecting method calls as 
        /// in project calls in stack traces
        /// </summary>
        public bool AutoDetectInProject { get; set; }

        public Func<Event, bool> BeforeNotifyFunc { get; set; }

        private List<string> notifyReleaseStages;

        private List<string> filePrefixes;

        private List<string> ignoreClasses;

        private List<string> projectNamespaces;

        public Configuration(string apiKey)
        {
            ApiKey = apiKey;
            AppVersion = "1.0.0";
            ReleaseStage = "Development";
            Metadata = new Metadata();
            UseSsl = true;
            AutoDetectInProject = true;
            UserId = Environment.UserName;
            LoggedOnUser = Environment.UserDomainName + "\\" + Environment.UserName;
            Context = null;
            Endpoint = "notify.bugsnag.com";
            filePrefixes = new List<string>();
            ignoreClasses = new List<string>();
            projectNamespaces = new List<string>();
            notifyReleaseStages = null;
        }

        /// <summary>
        /// Specifies the release stages that notifications should be sent
        /// </summary>
        /// <param name="releaseStages">The stages to notify on</param>
        public void SetNotifyReleaseStages(params string[] releaseStages)
        {
            notifyReleaseStages = releaseStages.ToList();
        }

        /// <summary>
        /// Resets any restrictions and notifies on all release stages
        /// </summary>
        public void NotifyOnAllReleaseStages()
        {
            notifyReleaseStages = null;
        }

        /// <summary>
        /// Detects if the current release stage is a stage that should be notified on
        /// </summary>
        /// <returns>True if we should notify, otherwise false</returns>
        public bool IsNotifyReleaseStage()
        {
            // Notify if no restrictions have been set or the release stage hasn't been set
            if (notifyReleaseStages == null || ReleaseStage == null)
                return true;

            return notifyReleaseStages.Any(x => x == ReleaseStage);
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
        /// Sets the file prefixes that should be removed from frame file paths
        /// </summary>
        /// <param name="prefixes">The prefixes to remove</param>
        public void SetFilePrefix(params string[] prefixes)
        {
            filePrefixes = prefixes.ToList();
        }

        /// <summary>
        /// Removes all file prefixes from a filename
        /// </summary>
        /// <param name="fileName">The filename to modify</param>
        /// <returns>The filename with the prefixes removed</returns>
        public string RemoveFileNamePrefix(string fileName)
        {
            var result = fileName;
            if (result == null)
                return result;
            filePrefixes.ForEach(x => result = result.Replace(x, string.Empty));
            return result;
        }

        /// <summary>
        /// Sets the project namespaces used to detect local method calls
        /// </summary>
        /// <param name="namespaces">The project namespaces</param>
        public void SetProjectNamespaces(params string[] namespaces)
        {
            projectNamespaces = namespaces.ToList();
        }

        /// <summary>
        /// Indicates if a method name belongs to In Project namespaces
        /// </summary>
        /// <param name="fullMethodName">The fully qualified method name</param>
        /// <returns>True if it belongs to one of the project namespaces, otherwise false</returns>
        public bool IsInProjectNamespace(string fullMethodName)
        {
            return projectNamespaces.Any(x => fullMethodName.StartsWith(x, StringComparison.Ordinal));
        }

        public void SetIgnoreClasses(params string[] classNames)
        {
            ignoreClasses = classNames.ToList();
        }

        public bool IsClassToIgnore(string className)
        {
            return ignoreClasses.Contains(className);
        }
    }
}
