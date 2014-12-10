using System;
using System.Configuration;

namespace Bugsnag.ConfigurationStorage
{
    class XMLStorage : ConfigurationSection, IConfigurationStorage
    {
        //TODO:SM Add appropriate default values
        private static XMLStorage _storage = ConfigurationManager.GetSection("bugsnagConfig") as XMLStorage;

        public static XMLStorage Settings
        {
            get
            {
                return _storage;
            }
        }

        [ConfigurationProperty("apiKey", IsRequired = true, DefaultValue = "12345678901234567890123456789012")]
        [RegexStringValidator("^[a-fA-F0-9]{32}$")]
        public string ApiKey
        {
            get { return (String)this["apiKey"]; }
            set { this["apiKey"] = value; }
        }

        [ConfigurationProperty("appVersion", IsRequired = false, DefaultValue = null)]
        [StringValidator]
        public string AppVersion
        {
            get { return (String)this["appVersion"]; }
            set { this["appVersion"] = value; }
        }

        [ConfigurationProperty("releaseStage", IsRequired = false, DefaultValue = null)]
        [StringValidator]
        public string ReleaseStage
        {
            get { return (String)this["releaseStage"]; }
            set { this["releaseStage"] = value; }
        }

        [ConfigurationProperty("endpoint", IsRequired = false, DefaultValue = "notify.bugsnag.com")]
        [StringValidator]
        public string Endpoint
        {
            get { return (String)this["endpoint"]; }
            set { this["endpoint"] = value; }
        }

        [ConfigurationProperty("userId", IsRequired = false, DefaultValue = null)]
        [StringValidator]
        public string UserId
        {
            get { return (String)this["userId"]; }
            set { this["userId"] = value; }
        }

        [ConfigurationProperty("userEmail", IsRequired = false, DefaultValue = null)]
        [StringValidator]
        public string UserEmail
        {
            get { return (String)this["userEmail"]; }
            set { this["userEmail"] = value; }
        }

        [ConfigurationProperty("userName", IsRequired = false, DefaultValue = null)]
        [StringValidator]
        public string UserName
        {
            get { return (String)this["userName"]; }
            set { this["userName"] = value; }
        }

        [ConfigurationProperty("context", IsRequired = false, DefaultValue = null)]
        [StringValidator]
        public string Context
        {
            get { return (String)this["context"]; }
            set { this["context"] = value; }
        }

        [ConfigurationProperty("autoDetectInProject", IsRequired = false, DefaultValue = true)]
        public bool AutoDetectInProject
        {
            get 
            {
                bool? autoDetectInProject = (bool?)this["autoDetectInProject"];
                return autoDetectInProject.HasValue ? autoDetectInProject.Value : true;
            }
            set { this["autoDetectInProject"] = value; }
        }

        [ConfigurationProperty("notifyReleaseStages", IsRequired = false, DefaultValue = null)]
        public string[] NotifyReleaseStages
        {
            get
            {
                String notifyReleaseStages = this["notifyReleaseStages"] as String;
                return notifyReleaseStages != null ? notifyReleaseStages.Split(',') : null;
            }
            set { this["notifyReleaseStages"] = value; }
        }

        [ConfigurationProperty("filePrefixes", IsRequired = false, DefaultValue = null)]
        public string[] FilePrefixes
        {
            get
            {
                String filePrefixes = this["filePrefixes"] as String;
                return filePrefixes != null ? filePrefixes.Split(',') : new string[] { };
            }
            set { this["filePrefixes"] = value; }
        }

        [ConfigurationProperty("projectNamespaces", IsRequired = false, DefaultValue = null)]
        public string[] ProjectNamespaces
        {
            get
            {
                String projectNamespaces = this["projectNamespaces"] as String;
                return projectNamespaces != null ? projectNamespaces.Split(',') : new string[] { };
            }
            set { this["projectNamespaces"] = value; }
        }

        [ConfigurationProperty("ignoreClasses", IsRequired = false, DefaultValue = null)]
        public string[] IgnoreClasses
        {
            get
            {
                String ignoreClasses = this["ignoreClasses"] as String;
                return ignoreClasses != null ? ignoreClasses.Split(',') : new string[] { };
            }
            set { this["ignoreClasses"] = value; }
        }

        [ConfigurationProperty("metadataFilters", IsRequired = false, DefaultValue = null)]
        public string[] MetadataFilters
        {
            get
            {
                String filters = this["metadataFilters"] as String;
                return filters != null ? filters.Split(',') : new string[] { };
            }
            set { this["metadataFilters"] = value; }
        }

        [ConfigurationProperty("autoNotify", IsRequired = false, DefaultValue = true)]
        public bool AutoNotify
        {
            get
            {
                bool? autoNotify = (bool?)this["autoNotify"];
                return autoNotify.HasValue ? autoNotify.Value : true;
            }
            set { this["autoNotify"] = value; }
        }
    }
}
