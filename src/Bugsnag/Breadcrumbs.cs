using Bugsnag.Payload;
using System.Collections.Generic;

namespace Bugsnag
{
  public abstract class Breadcrumbs
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
        BreadcrumbCollection.Add(breadcrumb);
      }
    }

    public Breadcrumb[] Retrieve()
    {
      lock (_backingStoreLock)
      {
        return BreadcrumbCollection.ToArray();
      }
    }

    protected abstract List<Breadcrumb> BreadcrumbCollection { get; }
  }
}
