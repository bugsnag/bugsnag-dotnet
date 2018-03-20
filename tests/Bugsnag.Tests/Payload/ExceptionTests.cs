using System.Linq;
using System.Threading.Tasks;
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

    public class AggregateExceptions
    {
      Exception[] exceptions;

      public AggregateExceptions()
      {
        Exceptions exceptions = null;
        var exceptionsToThrow = new[] { new System.Exception(), new System.DllNotFoundException() };
        var tasks = exceptionsToThrow.Select(e => Task.Run(() => { throw e; })).ToArray();

        try
        {
          Task.WaitAll(tasks);
        }
        catch (System.Exception exception)
        {
          exceptions = new Exceptions(exception, 0);
        }

        this.exceptions = exceptions.ToArray();
      }

      [Fact]
      public void ContainsDllNotFoundException()
      {
        Assert.Contains(exceptions, exception => exception.ErrorClass == "System.DllNotFoundException");
      }

      [Fact]
      public void ContainsException()
      {
        Assert.Contains(exceptions, exception => exception.ErrorClass == "System.Exception");
      }

      [Fact]
      public void AggregateExceptionIsLast()
      {
        var lastException = exceptions.Last();
        Assert.Equal("System.AggregateException", lastException.ErrorClass);
      }
    }
  }
}
