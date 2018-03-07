using System.Linq;
using Xunit;

namespace Bugsnag.Tests
{
  public class BreadcrumbsTests
  {
    [Fact]
    public void RestrictsMaxNumberOfBreadcrumbs()
    {
      var breadcrumbs = new Breadcrumbs(new Configuration { MaximumBreadcrumbs = 25 });

      for (int i = 0; i < 30; i++)
      {
        breadcrumbs.Leave($"{i}");
      }

      Assert.Equal(25, breadcrumbs.Retrieve().Count());
    }

    [Fact]
    public void WhenRetrievingBreadcrumbsCorrectNumberIsReturned()
    {
      var breadcrumbs = new Breadcrumbs(new Configuration { MaximumBreadcrumbs = 25 });

      for (int i = 0; i < 10; i++)
      {
        breadcrumbs.Leave($"{i}");
      }

      Assert.Equal(10, breadcrumbs.Retrieve().Count());
    }

    [Fact]
    public void CorrectBreadcrumbsAreReturned()
    {
      var breadcrumbs = new Breadcrumbs(new Configuration { MaximumBreadcrumbs = 5 });

      for (int i = 0; i < 10; i++)
      {
        breadcrumbs.Leave($"{i}");
      }

      var breadcrumbNames = breadcrumbs.Retrieve().Select(b => b.Name);

      Assert.Equal(new string[] { "5", "6", "7", "8", "9" }, breadcrumbNames);
    }
  }
}
