using System.Collections;
using System.Collections.Generic;

namespace Bugsnag
{
  /// <summary>
  /// Used by Bugsnag clients to store information in a list.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class InMemoryStorage<T> : IStorage<T>
  {
    private readonly List<T> _backingStore;

    public InMemoryStorage()
    {
      _backingStore = new List<T>();
    }

    public void Add(T item)
    {
      _backingStore.Add(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return _backingStore.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
