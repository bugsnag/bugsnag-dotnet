#if NET35
using System;

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
