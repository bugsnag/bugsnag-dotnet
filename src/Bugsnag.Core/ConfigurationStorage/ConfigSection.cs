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
            get { return _apiKey == null ? (String)this["apiKey"] : _apiKey; }
            set { _apiKey = value; }
        }

        private string _appVersion;
        [ConfigurationProperty("appVersion", IsRequired = false)]
        [StringValidator]
        public string AppVersion
        {
            get { return _appVersion == null ? (String)this[new ConfigurationProperty("appVersion", typeof(string), null)] : _appVersion; }
            set { _appVersion = value; }
        }

        private string _releaseStage;
        [ConfigurationProperty("releaseStage", IsRequired = false)]
        [StringValidator]
        public string ReleaseStage
        {
            get { return _releaseStage == null ? (String)this[new ConfigurationProperty("releaseStage", typeof(string), null)] : _releaseStage; }
            set { _releaseStage = value; }
        }

        private string _endpoint;
        [ConfigurationProperty("endpoint", IsRequired = false, DefaultValue = "notify.bugsnag.com")]
        [StringValidator]
        public string Endpoint
        {
            get { return _endpoint == null ? (String)this["endpoint"] : _endpoint; }
            set { _endpoint = value; }
        }

        private string _userId;
        [ConfigurationProperty("userId", IsRequired = false)]
        [StringValidator]
        public string UserId
        {
            get { return _userId == null ? (String)this[new ConfigurationProperty("userId", typeof(string), null)] : _userId; }
            set { _userId = value; }
        }

        private string _userEmail;
        [ConfigurationProperty("userEmail", IsRequired = false)]
        [StringValidator]
        public string UserEmail
        {
            get { return _userEmail == null ? (String)this[new ConfigurationProperty("userEmail", typeof(string), null)] : _userEmail; }
            set { _userEmail = value; }
        }

        private string _userName;
        [ConfigurationProperty("userName", IsRequired = false)]
        [StringValidator]
        public string UserName
        {
            get { return _userName == null ? (String)this[new ConfigurationProperty("userName", typeof(string), null)] : _userName; }
            set { _userName = value; }
        }

        private string _context;
        [ConfigurationProperty("context", IsRequired = false)]
        [StringValidator]
        public string Context
        {
            get { return _context == null ? (String)this[new ConfigurationProperty("context", typeof(string), null)] : _context; }
            set { _context = value; }
        }

        private bool? _autoDetectInProject;
        [ConfigurationProperty("autoDetectInProject", IsRequired = false, DefaultValue = true)]
        public bool AutoDetectInProject
        {
            get
            {
                if (_autoDetectInProject.HasValue)
                {
                    return _autoDetectInProject.Value;
                }
                else
                {
                    _autoDetectInProject = (bool?)this["autoDetectInProject"];
                    return _autoDetectInProject.HasValue ? _autoDetectInProject.Value : true;
                }
            }
            set { _autoDetectInProject = value; }
        }

        private bool? _autoNotify;
        [ConfigurationProperty("autoNotify", IsRequired = false, DefaultValue = true)]
        public bool AutoNotify
        {
            get
            {
                if (_autoNotify.HasValue)
                {
                    return _autoNotify.Value;
                }
                else
                {
                    _autoNotify = (bool?)this["autoNotify"];
                    return _autoNotify.HasValue ? _autoNotify.Value : true;
                }
            }
            set { _autoNotify = value; }
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
                if (_notifyReleaseStages == null)
                {
                    if (String.IsNullOrEmpty(NotifyReleaseStagesString))
                    {
                        return null;
                    }
                    else 
                    {
                        _notifyReleaseStages = NotifyReleaseStagesString.Split(',');
                        return _notifyReleaseStages;
                    }
                }
                else
                {
                    return _notifyReleaseStages;
                }
            }
            set { _notifyReleaseStages = value; }
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
                if (_filePrefixes == null)
                {
                    _filePrefixes = String.IsNullOrEmpty(FilePrefixesString) ? new string[] { } : FilePrefixesString.Split(',');
                    return _filePrefixes;
                }
                else
                {
                    return _filePrefixes;
                }
            }
            set { _filePrefixes = value; }
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
                if (_projectNamespaces == null)
                {
                    _projectNamespaces = String.IsNullOrEmpty(ProjectNamespacesString) ? new string[] { } : ProjectNamespacesString.Split(',');
                    return _projectNamespaces;
                }
                else
                {
                    return _projectNamespaces;
                }
            }
            set { _projectNamespaces = value; }
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
                if (_ignoreClasses == null)
                {
                    _ignoreClasses = String.IsNullOrEmpty(IgnoreClassesString) ? new string[] { } : IgnoreClassesString.Split(',');
                    return _ignoreClasses;
                }
                else
                {
                    return _ignoreClasses;
                }
            }
            set { _ignoreClasses = value; }
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
                if (_metadataFilters == null)
                {
                    _metadataFilters = String.IsNullOrEmpty(MetadataFiltersString) ? new string[] { } : MetadataFiltersString.Split(',');
                    return _metadataFilters;
                }
                else
                {
                    return _metadataFilters;
                }
            }
            set { _metadataFilters = value; }
        }
    }
}
