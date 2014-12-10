using System.Diagnostics;

namespace Bugsnag
{
    internal static class Logger
    {
        public static void Info(string message)
        {
            Debug.WriteLine(message, "bugsnag");
        }

        public static void Warning(string message)
        {
            Debug.WriteLine(message, "bugsnag");
        }

        public static void Error(string message)
        {
            Debug.WriteLine(message, "bugsnag");
        }
    }
}
