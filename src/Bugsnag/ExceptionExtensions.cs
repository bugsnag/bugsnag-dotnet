using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bugsnag
{
  public static class ExceptionExtensions
  {
    public static string FriendlyClassName(this Type type)
    {
      if (type.IsGenericParameter)
      {
        return type.Name;
      }

      if (!type.IsGenericType())
      {
        return type.FullName;
      }

      type = type.GetGenericTypeDefinition();

      var genericArguments = type.GetGenericArguments();

      var className = type.FullName.Substring(0, type.FullName.IndexOf("`"));

      var genericArgumentNames = string.Join(",", genericArguments.Select(a => a.FriendlyClassName()).ToArray());

      return string.Format("{0}<{1}>", className, genericArgumentNames);
    }

    public static string FriendlyMethodName(this MethodBase method)
    {
      var builder = new StringBuilder();

      if (method.DeclaringType != null)
      {
        builder.AppendFormat("{0}.", method.DeclaringType.FriendlyClassName());
      }

      builder.Append(method.Name);

      if (method is MethodInfo && method.IsGenericMethod)
      {
        var genericArguments = method.GetGenericArguments();
        var genericArgumentNames = genericArguments.Select(a => a.Name).ToArray();
        builder.AppendFormat("<{0}>", string.Join(", ", genericArgumentNames));
      }

      var parameters = method.GetParameters().Select(p => string.Format("{0} {1}", p.ParameterType.FriendlyClassName(), p.Name)).ToArray();

      builder.AppendFormat("({0})", string.Join(", ", parameters));

      return builder.ToString();
    }

    public static int FriendlyLineNumber(this StackFrame frame)
    {
      var lineNumber = frame.GetFileLineNumber();

      if (lineNumber == 0)
      {
        lineNumber = frame.GetILOffset();
      }

      return lineNumber;
    }
  }
}
