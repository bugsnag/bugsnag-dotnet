using System.Diagnostics;
using System.Text;

namespace Bugsnag
{
  public static class Serializer
  {
    public static string SerializeObject(object obj)
    {
      try
      {
        return SimpleJson.SimpleJson.SerializeObject(obj);
      }
      catch (System.Exception exception)
      {
        Trace.WriteLine(exception);
      }

      return null;
    }

    public static byte[] SerializeObjectToByteArray(object obj)
    {
      try
      {
        var serializedObject = SerializeObject(obj);
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
