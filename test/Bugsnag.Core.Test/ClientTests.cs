using Moq;
using System;
using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Core.Test
{
    public class ClientTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_ExceptionThrownWhenApiKeyIsInvalid(string invalidApiKey)
        {
            Assert.Throws<ArgumentException>(() => new Client(invalidApiKey));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_ExceptionThrownWhenApiKeyIsInvalidWhileConfiguringAutoNotify(string invalidApiKey)
        {
            Assert.Throws<ArgumentException>(() => new Client(invalidApiKey, true));
            Assert.Throws<ArgumentException>(() => new Client(invalidApiKey, false));
        }

        [Fact]
        public void Constructor_InstallExceptionHandlerIfAutoNotiyIsOn()
        {
            // Arrange
            var mockExceptionHandler = new Mock<IExceptionHandler>(MockBehavior.Strict);
            mockExceptionHandler.Setup(x => x.InstallHandler(It.IsAny<Action<Exception, bool>>()));

            // Act
            var testClient = new Client("123456", true, null, null, mockExceptionHandler.Object);

            // Assert
            mockExceptionHandler.VerifyAll();
        }

        [Fact]
        public void Constructor_DoNotInstallExceptionHandlerIfAutoNotiyIsOff()
        {
            // Arrange
            var mockExceptionHandler = new Mock<IExceptionHandler>(MockBehavior.Strict);

            // Act
            var testClient = new Client("123456", false, null, null, mockExceptionHandler.Object);

            // Assert
            mockExceptionHandler.VerifyAll();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void StartAutoNotify_InstallExceptionHandlerEverytime(bool autoNotify)
        {
            // Arrange
            var mockExceptionHandler = new Mock<IExceptionHandler>();
            var testClient = new Client("123456", autoNotify, null, null, mockExceptionHandler.Object);
            mockExceptionHandler.Setup(x => x.InstallHandler(It.IsAny<Action<Exception, bool>>()))
                                .Verifiable("Install was not called");

            // Act
            testClient.StartAutoNotify();

            // Assert
            mockExceptionHandler.VerifyAll();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void StopAutoNotify_UninstallExceptionHandlerEverytime(bool autoNotify)
        {
            // Arrange
            var mockExceptionHandler = new Mock<IExceptionHandler>();
            var testClient = new Client("123456", autoNotify, null, null, mockExceptionHandler.Object);
            mockExceptionHandler.Setup(x => x.UninstallHandler()).Verifiable("Uninstall was not called");

            // Act
            testClient.StopAutoNotify();

            // Assert
            mockExceptionHandler.VerifyAll();
        }


        [Fact]
        public void Notify_DoNothingIfTheEventIsNull()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>(MockBehavior.Strict);
            var testClient = new Client("123456", false, null, mockNotifier.Object, null);

            // Act
            testClient.Notify(null as Event);

            // Assert
            mockNotifier.VerifyAll();
        }

        [Fact]
        public void Notify_DoNothingIfWeDontNotifyOnThisReleaseStage()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>(MockBehavior.Strict);
            var mockConfig = new Mock<IConfiguration>();
            var testClient = new Client("123456", false, mockConfig.Object, mockNotifier.Object, null);
            var testExp = new StackOverflowException("Test Stack Overflow");
            var testEvent = new Event(testExp);

            mockConfig.Setup(x => x.IsNotifyReleaseStage()).Returns(false);

            // Act
            testClient.Notify(testEvent);

            // Assert
            mockNotifier.VerifyAll();
        }

        [Fact]
        public void Notify_CheckThatBeforeNotifyCallBackIsCalled()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>();
            var mockConfig = new Mock<IConfiguration>();
            var testClient = new Client("123456", false, mockConfig.Object, mockNotifier.Object, null);
            var testExp = new StackOverflowException("Test Stack Overflow");
            var testEvent = new Event(testExp);

            var hasCallbackRan = false;
            mockConfig.Setup(x => x.IsNotifyReleaseStage()).Returns(true);
            mockConfig.Setup(x => x.BeforeNotifyFunc).Returns(err =>
            {
                hasCallbackRan = true;
                Assert.Equal(testEvent, err);
                return true;
            });

            // Act
            testClient.Notify(testEvent);

            // Assert
            Assert.True(hasCallbackRan);
        }

        [Fact]
        public void Notify_DoNothingWhenCallbackReturnFalse()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>(MockBehavior.Strict);
            var mockConfig = new Mock<IConfiguration>();
            var testClient = new Client("123456", false, mockConfig.Object, mockNotifier.Object, null);
            var testExp = new StackOverflowException("Test Stack Overflow");
            var testEvent = new Event(testExp);

            var hasCallbackRan = false;
            mockConfig.Setup(x => x.IsNotifyReleaseStage()).Returns(true);
            mockConfig.Setup(x => x.BeforeNotifyFunc).Returns(err =>
            {
                hasCallbackRan = true;
                Assert.Equal(testEvent, err);
                return false;
            });

            // Act
            testClient.Notify(testEvent);

            // Assert
            Assert.True(hasCallbackRan);
            mockNotifier.VerifyAll();
        }

        [Fact]
        public void Notify_CheckExceptionClassIsToBeIgnoredSoDoNotNotify()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>(MockBehavior.Strict);
            var mockConfig = new Mock<IConfiguration>();
            var testClient = new Client("123456", false, mockConfig.Object, mockNotifier.Object, null);
            var testExp = new StackOverflowException("Test Stack Overflow");
            var testEvent = new Event(testExp);

            mockConfig.Setup(x => x.IsNotifyReleaseStage()).Returns(true);
            mockConfig.Setup(x => x.IsClassToIgnore("StackOverflowException")).Returns(true);

            // Act
            testClient.Notify(testEvent);

            // Assert
            mockNotifier.VerifyAll();
        }

        [Fact]
        public void Notify_DoNothingIfThereIsNoExceptionInTheEvent()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>(MockBehavior.Strict);
            var mockConfig = new Mock<IConfiguration>();
            var testClient = new Client("123456", false, mockConfig.Object, mockNotifier.Object, null);
            var testEvent = new Event(null);

            mockConfig.Setup(x => x.IsNotifyReleaseStage()).Returns(true);

            // Act
            testClient.Notify(testEvent);

            // Assert
            mockNotifier.VerifyAll();
        }

        [Fact]
        public void Notify_WithCallbackWhenErrorEventIsValidNotify()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>(MockBehavior.Strict);
            var mockConfig = new Mock<IConfiguration>();
            var testClient = new Client("123456", false, mockConfig.Object, mockNotifier.Object, null);
            var testExp = new StackOverflowException("Test Stack Overflow");
            var testEvent = new Event(testExp);

            var hasCallbackRan = false;
            mockConfig.Setup(x => x.IsNotifyReleaseStage()).Returns(true);
            mockConfig.Setup(x => x.IsClassToIgnore("StackOverflowException")).Returns(false);
            mockConfig.Setup(x => x.BeforeNotifyFunc).Returns(err =>
            {
                hasCallbackRan = true;
                Assert.Equal(testEvent, err);
                return true;
            });
            mockNotifier.Setup(x => x.Send(It.Is<Event>(y => y == testEvent)));

            // Act
            testClient.Notify(testEvent);

            // Assert
            Assert.True(hasCallbackRan);
            mockNotifier.VerifyAll();
        }

        [Fact]
        public void Notify_WithNoCallbackWhenErrorEventIsValidNotify()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>(MockBehavior.Strict);
            var mockConfig = new Mock<IConfiguration>();
            var testClient = new Client("123456", false, mockConfig.Object, mockNotifier.Object, null);
            var testExp = new StackOverflowException("Test Stack Overflow");
            var testEvent = new Event(testExp);

            mockConfig.Setup(x => x.IsNotifyReleaseStage()).Returns(true);
            mockConfig.Setup(x => x.IsClassToIgnore("StackOverflowException")).Returns(false);
            mockNotifier.Setup(x => x.Send(It.Is<Event>(y => y == testEvent)));

            // Act
            testClient.Notify(testEvent);

            // Assert
            mockNotifier.VerifyAll();
        }

        [Fact]
        public void Notify_CallingNotifyFromExceptionWithoutSeverityDefaultToWarning()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>(MockBehavior.Strict);
            var mockConfig = new Mock<IConfiguration>();
            var mockExceptionHandler = new Mock<IExceptionHandler>();
            var testClient = new Client("123456", false, mockConfig.Object, mockNotifier.Object, mockExceptionHandler.Object);
            var testExp = new StackOverflowException("Test Stack Overflow");

            // Set up the call so that we invoke the handler with our test exception immediately
            mockConfig.Setup(x => x.IsNotifyReleaseStage()).Returns(true);
            mockConfig.Setup(x => x.IsClassToIgnore("StackOverflowException")).Returns(false);
            mockNotifier.Setup(x => x.Send(It.Is<Event>(y => y.Exception == testExp &&
                                                             y.Severity == Severity.Warning)));

            // Act
            testClient.Notify(testExp);

            // Assert
            mockNotifier.VerifyAll();
        }

        [Fact]
        public void Notify_CallingNotifyFromExceptionAndMetaDataWithoutSeverityDefaultToWarning()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>(MockBehavior.Strict);
            var mockConfig = new Mock<IConfiguration>();
            var mockExceptionHandler = new Mock<IExceptionHandler>();
            var testClient = new Client("123456", false, mockConfig.Object, mockNotifier.Object, mockExceptionHandler.Object);
            var testExp = new StackOverflowException("Test Stack Overflow");
            var testMetadata = new Metadata();
            testMetadata.AddToTab("Tab 1", "Tab Key 1", "Tab Value 1");

            // Set up the call so that we invoke the handler with our test exception immediately
            mockConfig.Setup(x => x.IsNotifyReleaseStage()).Returns(true);
            mockConfig.Setup(x => x.IsClassToIgnore("StackOverflowException")).Returns(false);
            mockNotifier.Setup(x => x.Send(It.Is<Event>(y => y.Exception == testExp &&
                                                             y.Severity == Severity.Warning &&
                                                             y.Metadata.MetadataStore["Tab 1"]["Tab Key 1"] == "Tab Value 1")));

            // Act
            testClient.Notify(testExp, testMetadata);

            // Assert
            mockNotifier.VerifyAll();
        }

        [Fact]
        public void HandleDefaultException_NotifyIsCalledAsPartOfDefaultHandler()
        {
            // Arrange
            var mockNotifier = new Mock<INotifier>(MockBehavior.Strict);
            var mockConfig = new Mock<IConfiguration>();
            var mockExceptionHandler = new Mock<IExceptionHandler>();
            var testExp = new StackOverflowException("Test Stack Overflow");

            // Set up the call so that we invoke the handler with our test exception immediately
            mockExceptionHandler.Setup(x => x.InstallHandler(It.IsAny<Action<Exception, bool>>()))
                                .Callback<Action<Exception, bool>>(y => y.Invoke(testExp, true));

            mockConfig.Setup(x => x.IsNotifyReleaseStage()).Returns(true);
            mockConfig.Setup(x => x.IsClassToIgnore("StackOverflowException")).Returns(false);
            mockNotifier.Setup(x => x.Send(It.Is<Event>(y => y.Exception == testExp &&
                                                             y.IsRuntimeEnding == true &&
                                                             y.Severity == Severity.Error)));

            // Act
            var testClient = new Client("123456", true, mockConfig.Object, mockNotifier.Object, mockExceptionHandler.Object);

            // Assert
            mockNotifier.VerifyAll();
        }
    }
}
