using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Core.Test
{
    public class ConfigurationTests
    {
        [Theory]
        [InlineData("Random API Key")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        public void Constructor_CheckApiKeyIsRecordedCorrectly(string apiKey)
        {
            // Arrange + Act
            var testConfig = new Configuration(apiKey);

            // Assert
            Assert.Equal(apiKey, testConfig.ApiKey);
        }

        [Theory]
        [InlineData("another.point.com", true, "https://another.point.com/")]
        [InlineData("another.point.com", false, "http://another.point.com/")]
        [InlineData("end.random.com", true, "https://end.random.com/")]
        [InlineData("end.random.com", false, "http://end.random.com/")]
        public void EndpointUrl_UrlIsCreatedCorrectly(string endpoint, bool useSsl, string url)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.Endpoint = endpoint;
            testConfig.UseSsl = useSsl;

            // Assert
            Assert.Equal(url, testConfig.EndpointUrl.AbsoluteUri);
        }

        [Theory]
        [InlineData("stage 1")]
        [InlineData("stage 2")]
        [InlineData("stage 4")]
        [InlineData("stage 7")]
        [InlineData(null)]
        public void ReleaseStage_IdentifiesReleaseStagesToNotifyOn(string stage)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.SetNotifyReleaseStages("stage 1", "stage 2", "stage 4", "stage 7");
            testConfig.ReleaseStage = stage;

            // Assert
            Assert.True(testConfig.IsNotifyReleaseStage());
        }

        [Theory]
        [InlineData("stage 3")]
        [InlineData("stage 5")]
        [InlineData("stage 6")]
        [InlineData("stage 8")]
        [InlineData("")]
        public void ReleaseStage_IdentifiesReleaseStagesToNotNotifyOn(string stage)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.SetNotifyReleaseStages("stage 1", "stage 2", "stage 4", "stage 7");
            testConfig.ReleaseStage = stage;

            // Assert
            Assert.False(testConfig.IsNotifyReleaseStage());
        }

        [Theory]
        [InlineData("stage 4")]
        [InlineData("stage 5")]
        [InlineData("stage 6")]
        [InlineData("")]
        public void ReleaseStage_ClearingReleaseStagesCauseAllStagesToNotify(string stage)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.SetNotifyReleaseStages("stage 1", "stage 2", "stage 3");
            testConfig.ReleaseStage = stage;
            Assert.False(testConfig.IsNotifyReleaseStage());
            testConfig.NotifyOnAllReleaseStages();

            // Assert
            Assert.True(testConfig.IsNotifyReleaseStage());
        }

        [Theory]
        [InlineData("stage 1")]
        [InlineData("stage 2")]
        [InlineData("stage 3")]
        [InlineData("")]
        public void ReleaseStage_AllReleaseStageNotifyByDefault(string stage)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.ReleaseStage = stage;

            // Assert
            Assert.True(testConfig.IsNotifyReleaseStage());
        }

        [Theory]
        [InlineData(@"C:\MyProj\file1.cs")]
        [InlineData(@"H:\Path\To\file2.cs")]
        [InlineData(@"E:\file3.cs")]
        [InlineData(null)]
        [InlineData("")]
        public void FilePrefixes_LeaveFilenamesUntouchedIfNotSet(string fileName)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            var actFileName = testConfig.RemoveFileNamePrefix(fileName);
            testConfig.SetFilePrefix(@"C:\MyProj\", @"H:\", @"C:\MyOtherProj\Data");
            testConfig.SetFilePrefix();
            var actFileNameAfterReset = testConfig.RemoveFileNamePrefix(fileName);

            // Assert
            Assert.Equal(fileName, actFileName);
            Assert.Equal(fileName, actFileNameAfterReset);
        }

        [Theory]
        [InlineData(@"C:\MyProj\file1.cs", @"file1.cs")]
        [InlineData(@"H:\Path\To\file2.cs", @"Path\To\file2.cs")]
        [InlineData(@"E:\file3.cs", @"E:\file3.cs")]
        [InlineData(null, null)]
        [InlineData("", "")]
        public void FilePrefixes_RemovePrefixesSuccessfully(string fileName, string expFileName)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.SetFilePrefix(@"C:\MyProj\", @"H:\", @"C:\MyOtherProj\Data");
            var actFileName = testConfig.RemoveFileNamePrefix(fileName);

            // Assert
            Assert.Equal(expFileName, actFileName);
        }

        [Theory]
        [InlineData("ComA.ComB.Call()")]
        [InlineData("ComC.MyCall()")]
        [InlineData("ComD.ComA.ComB.Call()")]
        [InlineData(null)]
        [InlineData("")]
        public void ProjectNamespaces_AllMethodsAreNotInNamespaceIfNotBeenSet(string methodName)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            var actInProject = testConfig.IsInProjectNamespace(methodName);
            testConfig.SetProjectNamespaces("ComA", "ComC");
            testConfig.SetProjectNamespaces();
            var actInProjectAfterReset = testConfig.IsInProjectNamespace(methodName);

            // Assert
            Assert.False(actInProject);
            Assert.False(actInProjectAfterReset);
        }

        [Theory]
        [InlineData("ComA.ComB.Call()", true)]
        [InlineData("ComC.MyCall()", true)]
        [InlineData("ComD.ComA.ComB.Call()", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void ProjectNamespaces_ProjectMethodCallsAreIdentifiedCorrectly(string methodName, bool expInProject)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.SetProjectNamespaces("ComA", "ComC");
            var actInProject = testConfig.IsInProjectNamespace(methodName);

            // Assert
            Assert.Equal(expInProject, actInProject);
        }

        [Theory]
        [InlineData("Class1")]
        [InlineData("Class2")]
        [InlineData("Class3")]
        [InlineData("Class4")]
        [InlineData("Class5")]
        [InlineData(null)]
        [InlineData("")]
        public void IgnoreClasses_AllClassesAreNotIgnoredIfNotBeenSet(string className)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            var actIgnoreClass = testConfig.IsClassToIgnore(className);
            testConfig.SetIgnoreClasses("Class2", "Class5");
            testConfig.SetIgnoreClasses();
            var actIgnoreClassAfterReset = testConfig.IsClassToIgnore(className);

            // Assert
            Assert.False(actIgnoreClass);
            Assert.False(actIgnoreClassAfterReset);
        }

        [Theory]
        [InlineData("Class1", false)]
        [InlineData("Class2", true)]
        [InlineData("Class3", false)]
        [InlineData("Class4", false)]
        [InlineData("Class5", true)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void IgnoreClasses_ClassesToIgnoreAreIdentifiedCorrectly(string className, bool expIgnoreClass)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.SetIgnoreClasses("Class2", "Class5");
            var actIgnoreClass = testConfig.IsClassToIgnore(className);

            // Assert
            Assert.Equal(expIgnoreClass, actIgnoreClass);
        }

        [Theory]
        [InlineData("Entry1")]
        [InlineData("Entry2")]
        [InlineData("Entry3")]
        [InlineData("Entry4")]
        [InlineData("Entry5")]
        [InlineData(null)]
        [InlineData("")]
        public void Filters_AllEntriesAreNotFilteredIfNotBeenSet(string entryKey)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            var actFilterEntry = testConfig.IsEntryFiltered(entryKey);
            testConfig.SetFilters("Class2", "Class5");
            testConfig.SetIgnoreClasses();
            var actFilterEntryAfterReset = testConfig.IsEntryFiltered(entryKey);

            // Assert
            Assert.False(actFilterEntry);
            Assert.False(actFilterEntryAfterReset);
        }

        [Theory]
        [InlineData("Entry1", true)]
        [InlineData("Entry2", false)]
        [InlineData("Entry3", true)]
        [InlineData("Entry4", false)]
        [InlineData("Entry5", true)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void Filters_EntriesToFilterAreIdentifiedCorrectly(string entryKey, bool expFilterEntry)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.SetFilters("Entry1", "Entry3", "Entry5");
            var actFilterEntry = testConfig.IsEntryFiltered(entryKey);

            // Assert
            Assert.Equal(expFilterEntry, actFilterEntry);
        }

        [Fact]
        public void RunBeforeNotifyCallbacks_WillReturnIfCallbackExceptions()
        {
            // Arrange
            var testConfig = new Configuration("123456");
            var testExp = new System.Exception("Test Stack Overflow");
            var testEvent = new Event(testExp);

            bool callbackHasRun = false;
            testConfig.BeforeNotify(err =>
            {
                callbackHasRun = true;
                Assert.Equal(testEvent, err);
                throw new System.AccessViolationException("Invalid Access");
            });

            // Act
            bool returnValue = testConfig.RunBeforeNotifyCallbacks(testEvent);

            // Assert
            // TODO Check logger has exception details when logger has been added
            Assert.True(returnValue);
            Assert.True(callbackHasRun);
        }
    }
}
