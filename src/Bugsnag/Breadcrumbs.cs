using Bugsnag.Payload;
using System.Collections.Generic;

namespace Bugsnag
{
  /// <summary>
  /// Represents a collection of breadcrumbs
  /// </summary>
  public interface IBreadcrumbs
  {
    /// <summary>
    /// Add a breadcrumb to the collection
    /// </summary>
    /// <param name="message"></param>
    void Leave(string message);

    /// <summary>
    /// Add a breadcrumb to the collection with the specified type and metadata
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <param name="metadata"></param>
    void Leave(string message, BreadcrumbType type, IDictionary<string, string> metadata);

    /// <summary>
    /// Add a pre assembled breadcrumb to the collection.
    /// </summary>
    /// <param name="breadcrumb"></param>
    void Leave(Breadcrumb breadcrumb);

    /// <summary>
    /// Retrieve the collection of left breadcrumbs.
    /// </summary>
    /// <returns></returns>
    IEnumerable<Breadcrumb> Retrieve();
  }

  /// <summary>
  /// Implements an in memory collection of breadcrumbs. Keeping a configurable
  /// number of the most recently added breadcrumbs.
  /// </summary>
  public class Breadcrumbs : IBreadcrumbs
  {
    private readonly object _lock = new object();
    private readonly int _maximumBreadcrumbs;
    private readonly List<Breadcrumb> _breadcrumbs;

    /// <summary>
    /// Constructs a collection of breadcrumbs
    /// </summary>
    /// <param name="configuration"></param>
    public Breadcrumbs(IConfiguration configuration)
    {
      _maximumBreadcrumbs = configuration.MaximumBreadcrumbs;
      _breadcrumbs = new List<Breadcrumb>();
    }

    /// <summary>
    /// Add a breadcrumb to the collection using Manual type and no metadata.
    /// </summary>
    /// <param name="message"></param>
    public void Leave(string message)
    {
      Leave(message, BreadcrumbType.Manual, null);
    }

    /// <summary>
    /// Add a breadcrumb to the collection with the specified type and metadata
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <param name="metadata"></param>
    public void Leave(string message, BreadcrumbType type, IDictionary<string, string> metadata)
    {
      Leave(new Breadcrumb(message, type, metadata));
    }

    /// <summary>
    /// Add a pre assembled breadcrumb to the collection.
    /// </summary>
    /// <param name="breadcrumb"></param>
    public void Leave(Breadcrumb breadcrumb)
    {
      if (breadcrumb == null || _maximumBreadcrumbs < 1)
      {
        return;
      }

      lock (_lock)
      {
        if (_breadcrumbs.Count == _maximumBreadcrumbs)
        {
          _breadcrumbs.RemoveAt(0);
        }

        _breadcrumbs.Add(breadcrumb);
      }
    }

    /// <summary>
    /// Retrieve the collection of breadcrumbs at this point in time.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Breadcrumb> Retrieve()
    {
      lock (_lock)
      {
        return _breadcrumbs.ToArray();
      }
    }
  }
}
