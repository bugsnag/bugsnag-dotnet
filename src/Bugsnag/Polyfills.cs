using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
      try
      {
        return File.ReadLines(file);
      }
      catch (System.Exception exception)
      {
        Trace.WriteLine(exception);
        return Enumerable.Empty<string>();
      }
#endif
    }
  }

  public static class String
  {
    public static bool IsNullOrWhiteSpace(string s)
    {
#if NET35
      if (s == null) return true;

      for (int i = 0; i < s.Length; i++)
      {
        if (!Char.IsWhiteSpace(s[i])) return false;
      }

      return true;
#else
      return System.String.IsNullOrWhiteSpace(s);
#endif
    }
  }
}

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
