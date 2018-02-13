using System.Linq;
using Bugsnag.Payload;
using Xunit;

namespace Bugsnag.Tests.Payload
{
  public class ExceptionTests
  {
    [Fact]
    public void CorrectNumberOfExceptions()
    {
      var exception = new System.Exception("oh noes!");

      var exceptions = new Exceptions(exception, 5);

      Assert.Single(exceptions);
    }

    [Fact]
    public void IncludeInnerExceptions()
    {
      var innerException = new System.Exception();
      var exception = new System.Exception("oh noes!", innerException);

      var exceptions = new Exceptions(exception, 5);

      Assert.Equal(2, exceptions.Count());
    }
  }
}
