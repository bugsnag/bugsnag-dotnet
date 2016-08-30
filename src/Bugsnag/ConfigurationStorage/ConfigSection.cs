using System;
using System.Configuration;

namespace Bugsnag.ConfigurationStorage
{
    class ConfigSection : ConfigurationSection, IConfigurationStorage
    {
        private static ConfigSection _storage = ConfigurationManager.GetSection("bugsnagConfig") as ConfigSection;

        public static ConfigSection Settings
        {
            get
            {
                return _storage;
            }
        }

        private string _apiKey;
        [ConfigurationProperty("apiKey", IsRequired = true, DefaultValue = "12345678901234567890123456789012")]
        [RegexStringValidator("^[a-fA-F0-9]{32}$")]
        public string ApiKey
        {
            get { return this._apiKey == null ? (String)this["apiKey"] : this._apiKey; }
            set { this._apiKey = value; }
        }

        private string _appVersion;
        [ConfigurationProperty("appVersion", IsRequired = false)]
        [StringValidator]
        public string AppVersion
        {
            get { return this._appVersion == null ? (String)this[new ConfigurationProperty("appVersion", typeof(string), null)] : this._appVersion; }
            set { this._appVersion = value; }
        }

        private string _releaseStage;
        [ConfigurationProperty("releaseStage", IsRequired = false)]
        [StringValidator]
        public string ReleaseStage
        {
            get { return this._releaseStage == null ? (String)this[new ConfigurationProperty("releaseStage", typeof(string), null)] : this._releaseStage; }
            set { this._releaseStage = value; }
        }

        private string _endpoint;
        [ConfigurationProperty("endpoint", IsRequired = false, DefaultValue = "https://notify.bugsnag.com")]
        [StringValidator]
        public string Endpoint
        {
            get { return this._endpoint == null ? (String)this["endpoint"] : this._endpoint; }
            set { this._endpoint = value; }
        }

        private string _userId;
        [ConfigurationProperty("userId", IsRequired = false)]
        [StringValidator]
        public string UserId
        {
            get { return this._userId == null ? (String)this[new ConfigurationProperty("userId", typeof(string), null)] : this._userId; }
            set { this._userId = value; }
        }

        private string _userEmail;
        [ConfigurationProperty("userEmail", IsRequired = false)]
        [StringValidator]
        public string UserEmail
        {
            get { return this._userEmail == null ? (String)this[new ConfigurationProperty("userEmail", typeof(string), null)] : this._userEmail; }
            set { this._userEmail = value; }
        }

        private string _userName;
        [ConfigurationProperty("userName", IsRequired = false)]
        [StringValidator]
        public string UserName
        {
            get { return this._userName == null ? (String)this[new ConfigurationProperty("userName", typeof(string), null)] : this._userName; }
            set { this._userName = value; }
        }

        private string _context;
        [ConfigurationProperty("context", IsRequired = false)]
        [StringValidator]
        public string Context
        {
            get { return this._context == null ? (String)this[new ConfigurationProperty("context", typeof(string), null)] : this._context; }
            set { this._context = value; }
        }

        private bool? _autoDetectInProject;
        [ConfigurationProperty("autoDetectInProject", IsRequired = false, DefaultValue = true)]
        public bool AutoDetectInProject
        {
            get
            {
                if (this._autoDetectInProject.HasValue)
                {
                    return this._autoDetectInProject.Value;
                }
                else
                {
                    this._autoDetectInProject = (bool?)this["autoDetectInProject"];
                    return this._autoDetectInProject.HasValue ? this._autoDetectInProject.Value : true;
                }
            }
            set { this._autoDetectInProject = value; }
        }

        private bool? _autoNotify;
        [ConfigurationProperty("autoNotify", IsRequired = false, DefaultValue = true)]
        public bool AutoNotify
        {
            get
            {
                if (this._autoNotify.HasValue)
                {
                    return this._autoNotify.Value;
                }
                else
                {
                    this._autoNotify = (bool?)this["autoNotify"];
                    return this._autoNotify.HasValue ? this._autoNotify.Value : true;
                }
            }
            set { this._autoNotify = value; }
        }

        [ConfigurationProperty("notifyReleaseStages", IsRequired = false, DefaultValue = "")]
        [StringValidator]
        private string NotifyReleaseStagesString
        {
            get { return (String)this["notifyReleaseStages"]; }
        }
        private string[] _notifyReleaseStages;
        public string[] NotifyReleaseStages
        {
            get
            {
                if (this._notifyReleaseStages == null)
                {
                    if (String.IsNullOrEmpty(this.NotifyReleaseStagesString))
                    {
                        return null;
                    }
                    else
                    {
                        this._notifyReleaseStages = this.NotifyReleaseStagesString.Split(',');
                        return this._notifyReleaseStages;
                    }
                }
                else
                {
                    return this._notifyReleaseStages;
                }
            }
            set { this._notifyReleaseStages = value; }
        }

        [ConfigurationProperty("filePrefixes", IsRequired = false, DefaultValue = "")]
        [StringValidator]
        private string FilePrefixesString
        {
            get { return (String)this["filePrefixes"]; }
        }
        private string[] _filePrefixes;
        public string[] FilePrefixes
        {
            get
            {
                if (this._filePrefixes == null)
                {
                    this._filePrefixes = String.IsNullOrEmpty(this.FilePrefixesString) ? new string[] { } : this.FilePrefixesString.Split(',');
                    return this._filePrefixes;
                }
                else
                {
                    return this._filePrefixes;
                }
            }
            set { this._filePrefixes = value; }
        }

        [ConfigurationProperty("projectNamespaces", IsRequired = false, DefaultValue = "")]
        [StringValidator]
        private string ProjectNamespacesString
        {
            get { return (String)this["projectNamespaces"]; }
        }
        private string[] _projectNamespaces;
        public string[] ProjectNamespaces
        {
            get
            {
                if (this._projectNamespaces == null)
                {
                    this._projectNamespaces = String.IsNullOrEmpty(this.ProjectNamespacesString) ? new string[] { } : this.ProjectNamespacesString.Split(',');
                    return this._projectNamespaces;
                }
                else
                {
                    return this._projectNamespaces;
                }
            }
            set { this._projectNamespaces = value; }
        }

        [ConfigurationProperty("ignoreClasses", IsRequired = false, DefaultValue = "")]
        [StringValidator]
        private string IgnoreClassesString
        {
            get { return (String)this["ignoreClasses"]; }
        }
        private string[] _ignoreClasses;
        public string[] IgnoreClasses
        {
            get
            {
                if (this._ignoreClasses == null)
                {
                    this._ignoreClasses = String.IsNullOrEmpty(this.IgnoreClassesString) ? new string[] { } : this.IgnoreClassesString.Split(',');
                    return this._ignoreClasses;
                }
                else
                {
                    return this._ignoreClasses;
                }
            }
            set { this._ignoreClasses = value; }
        }

        [ConfigurationProperty("metadataFilters", IsRequired = false, DefaultValue = "")]
        [StringValidator]
        private string MetadataFiltersString
        {
            get { return (String)this["metadataFilters"]; }
        }
        private string[] _metadataFilters;
        public string[] MetadataFilters
        {
            get
            {
                if (this._metadataFilters == null)
                {
                    this._metadataFilters = String.IsNullOrEmpty(this.MetadataFiltersString) ? new string[] { } : this.MetadataFiltersString.Split(',');
                    return this._metadataFilters;
                }
                else
                {
                    return this._metadataFilters;
                }
            }
            set { this._metadataFilters = value; }
        }

        private bool? _storeOfflineErrors;
        [ConfigurationProperty("storeOfflineErrors", IsRequired = false, DefaultValue = false)]
        public bool StoreOfflineErrors
        {
            get
            {
                if (this._storeOfflineErrors.HasValue)
                {
                    return this._storeOfflineErrors.Value;
                }
                else
                {
                    this._storeOfflineErrors = (bool?)this["storeOfflineErrors"];
                    return this._storeOfflineErrors.HasValue ? this._storeOfflineErrors.Value : false;
                }
            }
            set { this._storeOfflineErrors = value; }
        }
    }
}
