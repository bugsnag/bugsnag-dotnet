using System.Collections.Generic;
using Bugsnag.Payload;

namespace Bugsnag
{
  /// <summary>
  /// Used by Bugsnag clients to store information in a list.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class InMemoryBreadcrumbs : Breadcrumbs
  {
    private readonly List<Breadcrumb> _backingStore;

    public InMemoryBreadcrumbs()
    {
      _backingStore = new List<Breadcrumb>();
    }

    protected override List<Breadcrumb> BreadcrumbCollection => _backingStore;
  }
}
