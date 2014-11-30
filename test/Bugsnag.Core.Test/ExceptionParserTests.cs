using Xunit;
using Xunit.Extensions;

namespace Bugsnag.Core.Test
{
    public class ExceptionParserTests
    {
        public class TestClass
        {

        }

        public int Calculate(string string1, int integer2)
        {
            return 0;
        }

        public void Addition(long long1, long long2)
        {
            return;
        }

        public TestClass Subtraction(params uint[] unsigned)
        {
            return null;
        }

        public void Multiplication<T>(TestClass testClass)
        {
            return;
        }

        public void Nothing()
        {
            return;
        }

        [Theory]
        [InlineData("Calculate", "Bugsnag.Core.Test.ExceptionParserTests.Calculate(String string1, Int32 integer2)")]
        [InlineData("Addition", "Bugsnag.Core.Test.ExceptionParserTests.Addition(Int64 long1, Int64 long2)")]
        [InlineData("Subtraction", "Bugsnag.Core.Test.ExceptionParserTests.Subtraction(UInt32[] unsigned)")]
        [InlineData("Nothing", "Bugsnag.Core.Test.ExceptionParserTests.Nothing()")]
        [InlineData("Multiplication", "Bugsnag.Core.Test.ExceptionParserTests.Multiplication(TestClass testClass)")]
        public void GenerateMethodSignature_GeneratesSignaturesCorrecly(string methodName, string expSignature)
        {
            // Arrange
            var method = typeof(ExceptionParserTests).GetMethod(methodName);

            // Act
            var actSignature = ExceptionParser.GenerateMethodSignature(method);

            // Assert
            Assert.Equal(expSignature, actSignature);


        }
    }
}
