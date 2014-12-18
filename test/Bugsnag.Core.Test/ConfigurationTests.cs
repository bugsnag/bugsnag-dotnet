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
        [InlineData("https://another.point.com/")]
        [InlineData("https://end.random.com/")]
        public void EndpointUrl_UrlIsCreatedCorrectly(string url)
        {
            // Arrange
            var testConfig = new Configuration("123456");

            // Act
            testConfig.Endpoint = url;

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
            testConfig.NotifyReleaseStages = new[] { "stage 1", "stage 2", "stage 4", "stage 7" };
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
            testConfig.NotifyReleaseStages = new[] { "stage 1", "stage 2", "stage 4", "stage 7" };
            testConfig.ReleaseStage = stage;

            // Assert
            Assert.False(testConfig.IsNotifyReleaseStage());
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

            // Assert
            Assert.Equal(fileName, actFileName);
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
            testConfig.FilePrefixes = new[] { @"C:\MyProj\", @"H:\", @"C:\MyOtherProj\Data" };
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

            // Assert
            Assert.False(actInProject);
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
            testConfig.ProjectNamespaces = new[] { "ComA", "ComC" };
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

            // Assert
            Assert.False(actIgnoreClass);
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
            testConfig.IgnoreClasses = new[] { "Class2", "Class5" };
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

            // Assert
            Assert.False(actFilterEntry);
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
            testConfig.MetadataFilters = new[] { "Entry1", "Entry3", "Entry5" };
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
            Assert.True(returnValue);
            Assert.True(callbackHasRun);
        }

        [Fact]
        public void RunBeforeNotifyCallbacks_WillRunMultipleCallbacks()
        {
            // Arrange
            var testConfig = new Configuration("123456");
            var testExp = new System.Exception("Test Stack Overflow");
            var testEvent = new Event(testExp);

            bool firstCallbackHasRun = false;
            bool secondCallbackHasRun = false;
            testConfig.BeforeNotify(err =>
            {
                firstCallbackHasRun = true;
                Assert.Equal(testEvent, err);
                return true;
            });
            testConfig.BeforeNotify(err =>
            {
                secondCallbackHasRun = true;
                Assert.Equal(testEvent, err);
                return true;
            });

            // Act
            bool returnValue = testConfig.RunBeforeNotifyCallbacks(testEvent);

            // Assert
            Assert.True(returnValue);
            Assert.True(firstCallbackHasRun);
            Assert.True(secondCallbackHasRun);
        }

        [Fact]
        public void RunBeforeNotifyCallbacks_WillStopRunning()
        {
            // Arrange
            var testConfig = new Configuration("123456");
            var testExp = new System.Exception("Test Stack Overflow");
            var testEvent = new Event(testExp);

            bool firstCallbackHasRun = false;
            bool secondCallbackHasRun = false;
            testConfig.BeforeNotify(err =>
            {
                firstCallbackHasRun = true;
                Assert.Equal(testEvent, err);
                return false;
            });
            testConfig.BeforeNotify(err =>
            {
                secondCallbackHasRun = true;
                Assert.Equal(testEvent, err);
                return true;
            });

            // Act
            bool returnValue = testConfig.RunBeforeNotifyCallbacks(testEvent);

            // Assert
            Assert.False(returnValue);
            Assert.True(firstCallbackHasRun);
            Assert.False(secondCallbackHasRun);
        }
    }
}
