using System;
using System.Configuration;

namespace Bugsnag.ConfigurationStorage
{
    class XMLStorage : ConfigurationSection, IConfigurationStorage
    {
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

        [ConfigurationProperty("appVersion", IsRequired = false)]
        [StringValidator]
        public string AppVersion
        {
            get { return (String)this[new ConfigurationProperty("appVersion", typeof(string), null)]; }
            set { this["appVersion"] = value; }
        }

        [ConfigurationProperty("releaseStage", IsRequired = false)]
        [StringValidator]
        public string ReleaseStage
        {
            get { return (String)this[new ConfigurationProperty("releaseStage", typeof(string), null)]; }
            set { this["releaseStage"] = value; }
        }

        [ConfigurationProperty("endpoint", IsRequired = false, DefaultValue = "notify.bugsnag.com")]
        [StringValidator]
        public string Endpoint
        {
            get { return (String)this["endpoint"]; }
            set { this["endpoint"] = value; }
        }

        [ConfigurationProperty("userId", IsRequired = false)]
        [StringValidator]
        public string UserId
        {
            get { return (String)this[new ConfigurationProperty("userId", typeof(string), null)]; }
            set { this["userId"] = value; }
        }

        [ConfigurationProperty("userEmail", IsRequired = false)]
        [StringValidator]
        public string UserEmail
        {
            get { return (String)this[new ConfigurationProperty("userEmail", typeof(string), null)]; }
            set { this["userEmail"] = value; }
        }

        [ConfigurationProperty("userName", IsRequired = false)]
        [StringValidator]
        public string UserName
        {
            get { return (String)this[new ConfigurationProperty("userName", typeof(string), null)]; }
            set { this["userName"] = value; }
        }

        [ConfigurationProperty("context", IsRequired = false)]
        [StringValidator]
        public string Context
        {
            get { return (String)this[new ConfigurationProperty("context", typeof(string), null)]; }
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

        [ConfigurationProperty("notifyReleaseStages", IsRequired = false, DefaultValue = "")]
        [StringValidator]
        public string NotifyReleaseStagesString
        {
            get { return (String)this["notifyReleaseStages"]; }
            set { this["notifyReleaseStages"] = value; }
        }
        public string[] NotifyReleaseStages
        {
            get { return String.IsNullOrEmpty(NotifyReleaseStagesString) ? null : NotifyReleaseStagesString.Split(','); }
            set { NotifyReleaseStagesString = String.Join(",", value); }
        }

        [ConfigurationProperty("filePrefixes", IsRequired = false, DefaultValue = "")]
        [StringValidator]
        public string FilePrefixesString
        {
            get { return (String)this["filePrefixes"]; }
            set { this["filePrefixes"] = value; }
        }
        public string[] FilePrefixes
        {
            get { return String.IsNullOrEmpty(FilePrefixesString) ? new string[] {} : FilePrefixesString.Split(','); }
            set { FilePrefixesString = String.Join(",", value); }
        }

        [ConfigurationProperty("projectNamespaces", IsRequired = false, DefaultValue = "")]
        [StringValidator]
        public string ProjectNamespacesString
        {
            get { return (String)this["projectNamespaces"]; }
            set { this["projectNamespaces"] = value; }
        }
        public string[] ProjectNamespaces
        {
            get { return String.IsNullOrEmpty(ProjectNamespacesString) ? new string[] { } : ProjectNamespacesString.Split(','); }
            set { ProjectNamespacesString = String.Join(",", value); }
        }

        [ConfigurationProperty("ignoreClasses", IsRequired = false, DefaultValue = "")]
        [StringValidator]
        public string IgnoreClassesString
        {
            get { return (String)this["ignoreClasses"]; }
            set { this["ignoreClasses"] = value; }
        }
        public string[] IgnoreClasses
        {
            get { return String.IsNullOrEmpty(IgnoreClassesString) ? new string[] { } : IgnoreClassesString.Split(','); }
            set { IgnoreClassesString = String.Join(",", value); }
        }

        [ConfigurationProperty("metadataFilters", IsRequired = false, DefaultValue = "")]
        [StringValidator]
        public string MetadataFiltersString
        {
            get { return (String)this["metadataFilters"]; }
            set { this["metadataFilters"] = value; }
        }
        public string[] MetadataFilters
        {
            get { return String.IsNullOrEmpty(MetadataFiltersString) ? new string[] { } : MetadataFiltersString.Split(','); }
            set { MetadataFiltersString = String.Join(",", value); }
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
