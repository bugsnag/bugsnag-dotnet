using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bugsnag
{
  public static class Serializer
  {
    public static string SerializeObject(object obj)
    {
      return SerializeObject(obj, new string[0]);
    }

    public static string SerializeObject(object obj, string[] filters)
    {
      var parsedFilters = filters != null ?
        filters.ToDictionary(m => m, m => true, StringComparer.OrdinalIgnoreCase) :
        new Dictionary<string, bool>();

      try
      {
        return SimpleJson.SimpleJson.SerializeObject(obj, parsedFilters);
      }
      catch (System.Exception exception)
      {
        Trace.WriteLine(exception);
      }

      return null;
    }

    public static byte[] SerializeObjectToByteArray(object obj)
    {
      return SerializeObjectToByteArray(obj, new string[0]);
    }

    public static byte[] SerializeObjectToByteArray(object obj, string[] filters)
    {
      try
      {
        var serializedObject = SerializeObject(obj, filters);
        return Encoding.UTF8.GetBytes(serializedObject);
      }
      catch (System.Exception exception)
      {
        Trace.WriteLine(exception);
      }

      return null;
    }
  }
}
