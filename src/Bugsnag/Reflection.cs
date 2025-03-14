using System;
using System.Reflection;

namespace Bugsnag
{
  /// <summary>
  /// Handle the reflection differences between the full .net framework and
  /// what is provided by netstandard
  /// </summary>
  public static class Reflection
  {
    public static Assembly GetAssembly(this Type type)
    {
      return type.GetTypeInfo().Assembly;
    }

    public static bool IsGenericType(this Type type)
    {
      return type.GetTypeInfo().IsGenericType;
    }

    public static Type[] GetGenericArguments(this Type type)
    {
      var typeInfo = type.GetTypeInfo();
      return typeInfo.IsGenericTypeDefinition
        ? typeInfo.GenericTypeParameters
        : typeInfo.GenericTypeArguments;
    }
  }
}
