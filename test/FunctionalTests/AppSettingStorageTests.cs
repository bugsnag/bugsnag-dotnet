using System;
using System.Collections.Specialized;
using Bugsnag.ConfigurationStorage;
using Xunit;

namespace Bugsnag.Test.FunctionalTests
{
    public class AppSettingStorageTests
    {
        [Fact]
        public void CheckThatAppSettingsUsedWhenNoCustomSettings()
        {
            // Arrange + Act
            var target = new AppSettingStorage();

            // Assert
            Assert.Equal<string>("2.0", target.AppVersion);
        }

        [Fact]
        public void CheckWithDefinedSettings()
        {
            // Arrange
            var values = new NameValueCollection();
            values["bugsnag.appVersion"] = "1.0";
            values["bugsnag.releaseStage"] = "staging";
            values["bugsnag.userId"] = "testUserId";
            values["bugsnag.userEmail"] = "testUserEmail";
            values["bugsnag.userName"] = "testUserName";
            values["bugsnag.context"] = "testContext";
            values["bugsnag.apiKey"] = "abc123";
            values["bugsnag.endpoint"] = "testEndpoint";
            values["bugsnag.autoDetectInProject"] = "true";
            values["bugsnag.autoNotify"] = "true";
            values["bugsnag.notifyReleaseStages"] = "a,b";
            values["bugsnag.filePrefixes"] = "c,d";
            values["bugsnag.projectNamespaces"] = "e,f";
            values["bugsnag.ignoreClasses"] = "g,h";
            values["bugsnag.metadataFilters"] = "i,j";

            // Act
            var target = new AppSettingStorage(values);

            // Assert
            Assert.Equal<string>(values["bugsnag.appVersion"], target.AppVersion);
            Assert.Equal<string>(values["bugsnag.releaseStage"], target.ReleaseStage);
            Assert.Equal<string>(values["bugsnag.userId"], target.UserId);
            Assert.Equal<string>(values["bugsnag.userEmail"], target.UserEmail);
            Assert.Equal<string>(values["bugsnag.userName"], target.UserName);
            Assert.Equal<string>(values["bugsnag.context"], target.Context);
            Assert.Equal<string>(values["bugsnag.apiKey"], target.ApiKey);
            Assert.Equal<string>(values["bugsnag.endpoint"], target.Endpoint);
            Assert.True(target.AutoDetectInProject);
            Assert.True(target.AutoNotify);
            Assert.Equal<string>(new string[] { "a", "b" }, target.NotifyReleaseStages);
            Assert.Equal<string>(new string[] { "c", "d" }, target.FilePrefixes);
            Assert.Equal<string>(new string[] { "e", "f" }, target.ProjectNamespaces);
            Assert.Equal<string>(new string[] { "g", "h" }, target.IgnoreClasses);
            Assert.Equal<string>(new string[] { "i", "j" }, target.MetadataFilters);
        }

        [Fact]
        public void CheckWithNoSettings()
        {
            // Arrange
            var values = new NameValueCollection();
            var baseStorage = new BaseStorage(null);
            
            // Act
            var target = new AppSettingStorage(values);

            // Assert
            Assert.Equal<string>(baseStorage.AppVersion, target.AppVersion);
            Assert.Equal<string>(baseStorage.ReleaseStage, target.ReleaseStage);
            Assert.Equal<string>(baseStorage.UserId, target.UserId);
            Assert.Equal<string>(baseStorage.UserEmail, target.UserEmail);
            Assert.Equal<string>(baseStorage.UserName, target.UserName);
            Assert.Equal<string>(baseStorage.Context, target.Context);
            Assert.Equal<string>(baseStorage.ApiKey, target.ApiKey);
            Assert.Equal<string>(baseStorage.Endpoint, target.Endpoint);
            Assert.Equal<bool>(baseStorage.AutoDetectInProject, target.AutoDetectInProject);
            Assert.Equal<bool>(baseStorage.AutoNotify, target.AutoNotify);
            Assert.Equal<string>(baseStorage.NotifyReleaseStages, target.NotifyReleaseStages);
            Assert.Equal<string>(baseStorage.FilePrefixes, target.FilePrefixes);
            Assert.Equal<string>(baseStorage.ProjectNamespaces, target.ProjectNamespaces);
            Assert.Equal<string>(baseStorage.IgnoreClasses, target.IgnoreClasses);
            Assert.Equal<string>(baseStorage.MetadataFilters, target.MetadataFilters);
        }

        [Fact]
        public void CheckChangedApiKey()
        {
            // Arrange
            string oldKey = "oldKey";
            string newKey = "newKey";
            var values = new NameValueCollection(); // Avoid appSettings to fall back to default values
            var target = new AppSettingStorage(values);

            // Act
            target.ApiKey = oldKey;
            string oldKeyValue = target.ApiKey;
            target.ApiKey = newKey;
            string newKeyValue = target.ApiKey;
            
            // Assert
            Assert.Equal<string>(oldKey, oldKeyValue);
            Assert.Equal<string>(newKey, newKeyValue);
        }
    }
}
