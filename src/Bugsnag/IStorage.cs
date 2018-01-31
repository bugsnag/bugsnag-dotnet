using System.Collections.Generic;

namespace Bugsnag
{
  /// <summary>
  /// Used by Bugsnag clients to store information that needs to be attached to
  /// Bugsnag error reports. Some frameworks may need to store this in a thread local
  /// or scoped on a per request basis for web frameworks.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IStorage<T> : IEnumerable<T>
  {
    void Add(T item);
  }
}
