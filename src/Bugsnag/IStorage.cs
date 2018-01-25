using System.Collections.Generic;

namespace Bugsnag
{
  public interface IStorage<T> : IEnumerable<T>
  {
    void Add(T item);
  }
}
