using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace Bugsnag.AspNet
{
  public class HttpContextStore<T> : IStorage<T>
  {
    private static object _lock = new object();

    private readonly string _key;

    [ThreadStatic]
    private static List<T> _threadStore;

    public HttpContextStore(string key)
    {
      _key = key;
    }

    public void Add(T item)
    {
      BackingStore.Add(item);
    }

    private IList<T> BackingStore
    {
      get
      {
        lock (_lock)
        {
          if (HttpContext.Current != null)
          {
            if (HttpContext.Current.Items[_key] == null)
            {
              HttpContext.Current.Items[_key] = new List<T>();
            }

            return HttpContext.Current.Items[_key] as List<T>;
          }
        }

        return ThreadStore;
      }
    }

    private IList<T> ThreadStore
    {
      get
      {
        if (_threadStore == null)
        {
          _threadStore = new List<T>();
        }

        return _threadStore;
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      return BackingStore.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
