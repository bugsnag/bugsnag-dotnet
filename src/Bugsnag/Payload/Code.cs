using Bugsnag.Polyfills;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Bugsnag.Payload
{
  /// <summary>
  /// TODO: Do we want to include this? It will only work in development so may
  /// be confusing for users why it doesn't work in other environments
  /// </summary>
  public class Code
  {
    private readonly StackFrame _stackFrame;
    private readonly int _sourceCodeLineCount;

    public Code(StackFrame stackFrame, int sourceCodeLineCount)
    {
      _stackFrame = stackFrame;
      _sourceCodeLineCount = sourceCodeLineCount;
    }

    public Dictionary<string, string> Display()
    {
      var file = _stackFrame.GetFileName();

      if (string.IsNullOrEmpty(file) || !File.Exists(file))
      {
        return null;
      }

      var line = _stackFrame.GetFileLineNumber();

      var startLineNumber = Math.Max(line - _sourceCodeLineCount, 1);
      var endLineNumber = line + _sourceCodeLineCount;
      var lineCounter = startLineNumber;
      return FileExtensions.ReadLines(file)
        .Skip(startLineNumber - 1)
        .Take(endLineNumber - startLineNumber + 1)
        .ToDictionary(k => (lineCounter++).ToString(), v => v);
    }
  }
}
