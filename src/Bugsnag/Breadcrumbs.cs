using Bugsnag.Payload;
using System.Collections.Generic;

namespace Bugsnag
{
  public interface IBreadcrumbs
  {
    void Leave(string message);

    void Leave(string message, BreadcrumbType type, IDictionary<string, string> metadata);

    void Leave(Breadcrumb breadcrumb);

    IEnumerable<Breadcrumb> Retrieve();
  }

  public class Breadcrumbs : IBreadcrumbs
  {
    private readonly object _lock = new object();
    private readonly int _maximumBreadcrumbs;
    private readonly Breadcrumb[] _breadcrumbs;
    private int _current;

    public Breadcrumbs(IConfiguration configuration)
    {
      _maximumBreadcrumbs = configuration.MaximumBreadcrumbs;
      _current = 0;
      _breadcrumbs = new Breadcrumb[_maximumBreadcrumbs];
    }

    public void Leave(string message)
    {
      Leave(message, BreadcrumbType.Manual, null);
    }

    public void Leave(string message, BreadcrumbType type, IDictionary<string, string> metadata)
    {
      Leave(new Breadcrumb(message, type, metadata));
    }

    public void Leave(Breadcrumb breadcrumb)
    {
      if (breadcrumb != null)
      {
        lock (_lock)
        {
          _breadcrumbs[_current] = breadcrumb;
          _current = (_current + 1) % _maximumBreadcrumbs;
        }
      }
    }

    public IEnumerable<Breadcrumb> Retrieve()
    {
      lock (_lock)
      {
        var numberOfBreadcrumbs = System.Array.IndexOf(_breadcrumbs, null);

        if (numberOfBreadcrumbs < 0) numberOfBreadcrumbs = _maximumBreadcrumbs;

        var breadcrumbs = new Breadcrumb[numberOfBreadcrumbs];

        for (int i = 0; i < numberOfBreadcrumbs; i++)
        {
          breadcrumbs[i] = _breadcrumbs[i];
        }

        return breadcrumbs;
      }
    }
  }
}
