using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bugsnag.Polyfills
{
  public static class FileExtensions
  {
    public static IEnumerable<string> ReadLines(string file)
    {
#if NET35
      return Enumerable.Empty<string>();
#else
      return File.ReadLines(file);
#endif
    }
  }
}

#if NET35
namespace Bugsnag
{
  /// <summary>
  /// Shim this method for net35, source taken from the reference source on GitHub.
  /// </summary>
  static class String
  {
    public static bool IsNullOrWhiteSpace(System.String value)
    {
      if (value == null) return true;

      for (int i = 0; i < value.Length; i++)
      {
        if (!Char.IsWhiteSpace(value[i])) return false;
      }

      return true;
    }
  }
}
#endif


#if NET35 || NET40
namespace System.Reflection
{
  public static class IntrospectionExtensions
  {
    public static TypeInfo GetTypeInfo(this Type type)
    {
      return new TypeInfo();
    }
  }

  public class TypeInfo
  {
    public Type[] GenericTypeArguments { get; } = new Type[0];

    public bool IsGenericType => false;
  }
}

namespace Microsoft.Extensions.FileProviders
{
  public interface IFileProvider
  {
    IFileInfo GetFileInfo(string path);
  }

  public interface IFileInfo
  {
    string PhysicalPath { get; }
    bool Exists { get; }
    Stream CreateReadStream();
  }
}
#endif
