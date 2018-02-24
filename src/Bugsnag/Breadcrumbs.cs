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
    private readonly List<Breadcrumb> _breadcrumbs;

    public Breadcrumbs()
    {
      _breadcrumbs = new List<Breadcrumb>();
    }

    private List<Breadcrumb> InternalBreadcrumbs => _breadcrumbs;

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
      lock (_lock)
      {
        if (breadcrumb != null)
        {
          InternalBreadcrumbs.Add(breadcrumb);
        }
      }
    }

    public IEnumerable<Breadcrumb> Retrieve()
    {
      lock (_lock)
      {
        return InternalBreadcrumbs.ToArray();
      }
    }
  }
}
