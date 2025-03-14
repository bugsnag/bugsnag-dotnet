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
      try
      {
        return File.ReadLines(file);
      }
      catch (System.Exception exception)
      {
        Trace.WriteLine(exception);
        return Enumerable.Empty<string>();
      }
    }
  }
}
