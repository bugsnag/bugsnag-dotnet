using System;
using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Clients.Test
{
    public class ClientTests
    {
        public const string TestApiKey = "ABCDEF1234567890ABCDEF1234567890";

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("123456")]
        [InlineData("MySuperLong32CharacterLengthZKey")]
        [InlineData("ABCDEF1234567890ABCDEF1234567890ABCDEF1234567890ABCDEF1234567890")]
        public void Constructor_ExceptionThrownWhenApiKeyIsInvalid(string invalidApiKey)
        {
            Assert.Throws<ArgumentException>(() => new BaseClient(invalidApiKey));
            Assert.Throws<ArgumentException>(() => new BaseClient(new ConfigurationStorage.BaseStorage(invalidApiKey)));
        }
    }
}
