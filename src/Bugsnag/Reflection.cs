using System;
using System.Reflection;

namespace Bugsnag
{
  public static class Reflection
  {
    public static Assembly GetAssembly(this Type type)
    {
#if (NET35 || NET40 || NET45)
      return type.Assembly;
#else
      return type.GetTypeInfo().Assembly;
#endif
    }

    public static bool IsGenericType(this Type type)
    {
#if (NET35 || NET40 || NET45)
      return type.IsGenericType;
#else
      return type.GetTypeInfo().IsGenericType;
#endif
    }

    public static Type[] GetGenericArguments(this Type type)
    {
#if NET35 || NET40 || NET45
      return type.GetGenericArguments();
#else
      return type.GetTypeInfo().GenericTypeParameters;
#endif
    }
  }
}
