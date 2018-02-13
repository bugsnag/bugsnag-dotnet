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

  public abstract class Breadcrumbs : IBreadcrumbs
  {
    private readonly object _backingStoreLock = new object();

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
      lock (_backingStoreLock)
      {
        if (breadcrumb != null)
        {
          BreadcrumbCollection.Add(breadcrumb);
        }
      }
    }

    public IEnumerable<Breadcrumb> Retrieve()
    {
      lock (_backingStoreLock)
      {
        return BreadcrumbCollection.ToArray();
      }
    }

    protected abstract List<Breadcrumb> BreadcrumbCollection { get; }
  }
}
