using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bugsnag.Payload
{
  public static class PayloadExtensions
  {
    /// <summary>
    /// Adds a key to the Bugsnag payload. If provided a null or empty string
    /// value will remove the key from the dictionary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddToPayload<T>(this Dictionary<string, T> dictionary, string key, T value)
    {
      if (value == null)
      {
        dictionary.Remove(key);
        return;
      }

      switch (value)
      {
        case System.String s:
          if (!String.IsNullOrWhiteSpace(s))
          {
            dictionary[key] = value;
          }
          else
          {
            dictionary.Remove(key);
          }
          break;
        default:
          dictionary[key] = value;
          break;
      }
    }

    public static U Get<T, U>(this Dictionary<T, U> dictionary, T key)
    {
      dictionary.TryGetValue(key, out U value);
      return value;
    }

    public static byte[] Serialize(this IDictionary dictionary)
    {
      byte[] data = null;

      try
      {
        var payload = SimpleJson.SimpleJson.SerializeObject(dictionary);
        data = System.Text.Encoding.UTF8.GetBytes(payload);
      }
      catch (System.Exception exception)
      {
        Trace.WriteLine(exception);
      }

      return data;
    }

    public static void FilterPayload(this IDictionary dictionary, string[] filters)
    {
      dictionary.FilterPayload(filters, new Dictionary<object, bool>());
    }

    public static void FilterPayload(this IDictionary dictionary, string[] filters, IDictionary seen)
    {
      if (seen.Contains(dictionary))
      {
        return;
      }

      seen.Add(dictionary, true);

      foreach (var key in filters)
      {
        if (key != null && dictionary.Contains(key))
        {
          dictionary[key] = "[Filtered]"; 
        }
      }

      foreach (DictionaryEntry k in dictionary)
      {
        switch (k.Value)
        {
          case string _:
            break;
          case Uri uri:
            uri.FilterUri(filters);
            break;
          case IDictionary subDictionary:
            subDictionary.FilterPayload(filters, seen);
            break;
          case IEnumerable enumerable:
            enumerable.FilterPayload(filters, seen);
            break;
        }
      }

      seen.Remove(dictionary);
    }

    public static void FilterPayload(this IEnumerable enumerable, string[] filters, IDictionary seen)
    {
      if (seen.Contains(enumerable))
      {
        return;
      }

      seen.Add(enumerable, true);

      foreach (var item in enumerable)
      {
        switch (item)
        {
          case string _:
            break;
          case Uri uri:
            uri.FilterUri(filters);
            break;
          case IDictionary dictionary:
            dictionary.FilterPayload(filters, seen);
            break;
          case IEnumerable subEnumerable:
            subEnumerable.FilterPayload(filters, seen);
            break;
        }
      }

      seen.Remove(enumerable);
    }

    public static void FilterUri(this Uri uri, string[] filters)
    {
      // need to figure out a good way to modify this, it is basically readonly at this point
    }
  }
}
