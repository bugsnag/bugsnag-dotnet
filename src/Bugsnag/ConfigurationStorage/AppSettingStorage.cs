using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Bugsnag.ConfigurationStorage
{
    /// <summary>
    /// Defines the app setting storage source
    /// </summary>
    public class AppSettingStorage : IConfigurationStorage
    {

        /// <summary>
        /// Defines the base configuration storage to manage default settings
        /// </summary>
        protected IConfigurationStorage baseStorage;
        
        /// <summary>
        /// Defines the available settings for the storage
        /// </summary>
        /// <remarks>
        /// Defaults to appSettings of application or web configuration
        /// </remarks>
        protected NameValueCollection settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingStorage" /> class.
        /// </summary>
        public AppSettingStorage() : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingStorage" /> class.
        /// </summary>
        /// <param name="defaultStorage">The default storage.</param>
        public AppSettingStorage(IConfigurationStorage defaultStorage) : this(defaultStorage, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingStorage" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public AppSettingStorage(NameValueCollection settings) : this(null, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingStorage" /> class.
        /// </summary>
        /// <param name="defaultStorage">The default storage.</param>
        /// <param name="settings">The settings.</param>
        public AppSettingStorage(IConfigurationStorage defaultStorage, NameValueCollection settings)
        {
            this.baseStorage = defaultStorage ?? new BaseStorage(null);
            this.settings = settings ?? System.Configuration.ConfigurationManager.AppSettings;
        }

        /// <summary>
        /// Gets the specified app setting configuration by key.
        /// </summary>
        /// <param name="key">The app setting key.</param>
        /// <returns>value for the app setting</returns>
        protected string Get(string key)
        {
            return settings[key];
        }

        /// <summary>
        /// Gets or sets the application version
        /// </summary>
        public string AppVersion
        {
            get
            {
                return this.Get("bugsnag.appVersion") ?? baseStorage.AppVersion;
            }
            set
            {
                baseStorage.AppVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the release stage of the application
        /// </summary>
        public string ReleaseStage
        {
            get
            {
                return this.Get("bugsnag.releaseStage") ?? baseStorage.ReleaseStage;
            }
            set
            {
                baseStorage.ReleaseStage = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier used to identify a user
        /// </summary>
        public string UserId
        {
            get
            {
                return this.Get("bugsnag.userId") ?? baseStorage.UserId;
            }
            set
            {
                baseStorage.UserId = value;
            }
        }

        /// <summary>
        /// Gets or sets the users email
        /// </summary>
        public string UserEmail
        {
            get
            {
                return this.Get("bugsnag.userEmail") ?? baseStorage.UserEmail;
            }
            set
            {
                baseStorage.UserEmail = value;
            }
        }

        /// <summary>
        /// Gets or sets the users human readable name
        /// </summary>
        public string UserName
        {
            get
            {
                return this.Get("bugsnag.userName") ?? baseStorage.UserName;
            }
            set
            {
                baseStorage.UserName = value;
            }
        }

        /// <summary>
        /// Gets or sets the context to apply to the subsequent notifications
        /// </summary>
        public string Context
        {
            get
            {
                return this.Get("bugsnag.context") ?? baseStorage.Context;
            }
            set
            {
                baseStorage.Context = value;
            }
        }

        /// <summary>
        /// Gets or sets the API key used to send notifications to a specific Bugsnag account
        /// </summary>
        public string ApiKey
        {
            get
            {
                return this.Get("bugsnag.apiKey") ?? baseStorage.ApiKey;
            }
            set
            {
                baseStorage.ApiKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the endpoint that defines where to send the notifications
        /// </summary>
        public string Endpoint
        {
            get
            {
                return this.Get("bugsnag.endpoint") ?? baseStorage.Endpoint;
            }
            set
            {
                baseStorage.Endpoint = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we are auto detecting method calls as 
        /// in project calls in stack traces
        /// </summary>
        public bool AutoDetectInProject
        {
            get
            {
                string rawValue = this.Get("bugsnag.autoDetectInProject");
                if (rawValue == null)
                    return baseStorage.AutoDetectInProject;
                else
                    return rawValue.Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            set
            {
                baseStorage.AutoDetectInProject = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether the notifier should register to automatically sent uncaught exceptions.
        /// </summary>
        public bool AutoNotify
        {
            get
            {
                string rawValue = this.Get("bugsnag.autoNotify");
                if (rawValue == null)
                    return baseStorage.AutoNotify;
                else
                    return rawValue.Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            set
            {
                baseStorage.AutoNotify = value;
            }
        }

        /// <summary>
        /// Gets or Sets the list of Release Stages to notify Bugsnag of errors in. If this is null
        /// then Bugsnag will notify on all Release Stages.
        /// </summary>
        public string[] NotifyReleaseStages
        {
            get
            {
                string rawValue = this.Get("bugsnag.notifyReleaseStages");
                if (rawValue == null)
                    return baseStorage.NotifyReleaseStages;
                else
                    return rawValue.Split(',');
            }
            set
            {
                baseStorage.NotifyReleaseStages = value;
            }
        }

        /// <summary>
        /// Gets or Sets a list of prefixes to strip from filenames in the stack trace. Helps
        /// with grouping errors from different build environments or machines by ensuring the stack
        /// trace looks similar.
        /// </summary>
        public string[] FilePrefixes
        {
            get
            {
                string rawValue = this.Get("bugsnag.filePrefixes");
                if (rawValue == null)
                    return baseStorage.FilePrefixes;
                else
                    return rawValue.Split(',');
            }
            set
            {
                baseStorage.FilePrefixes = value;
            }
        }

        /// <summary>
        /// Gets or Sets a list of namespaces to be considered in project. Helps with grouping
        /// as well as being highlighted in the Bugsnag dashboard.
        /// </summary>
        public string[] ProjectNamespaces
        {
            get
            {
                string rawValue = this.Get("bugsnag.projectNamespaces");
                if (rawValue == null)
                    return baseStorage.ProjectNamespaces;
                else
                    return rawValue.Split(',');
            }
            set
            {
                baseStorage.ProjectNamespaces = value;
            }
        }

        /// <summary>
        /// Gets or Sets the list of error classes not to be notified to Bugsnag.
        /// </summary>
        public string[] IgnoreClasses
        {
            get
            {
                string rawValue = this.Get("bugsnag.ignoreClasses");
                if (rawValue == null)
                    return baseStorage.IgnoreClasses;
                else
                    return rawValue.Split(',');
            }
            set
            {
                baseStorage.IgnoreClasses = value;
            }
        }

        /// <summary>
        /// Gets or Sets the list of keys not to be sent to Bugsnag. Add values to this list to
        /// prevent sensitive information being sent to Bugsnag.
        /// </summary>
        public string[] MetadataFilters
        {
            get
            {
                string rawValue = this.Get("bugsnag.metadataFilters");
                if (rawValue == null)
                    return baseStorage.MetadataFilters;
                else
                    return rawValue.Split(',');
            }
            set
            {
                baseStorage.MetadataFilters = value;
            }
        }
    }
}
