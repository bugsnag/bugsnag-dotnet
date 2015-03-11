using System.Diagnostics;

namespace Bugsnag
{
    /// <summary>
    /// Default logging to use in this Bugsnag library
    /// </summary>
    internal static class Logger
    {
        /// <summary>
        /// Record an informational message
        /// </summary>
        /// <param name="message">The info message</param>
        public static void Info(string message)
        {
            Debug.WriteLine("[INFO] " + message, "bugsnag");
        }

        /// <summary>
        /// Record a warning message
        /// </summary>
        /// <param name="message">The warning message</param>
        public static void Warning(string message)
        {
            Debug.WriteLine("[WARN] " + message, "bugsnag");
        }

        /// <summary>
        /// Record an error message
        /// </summary>
        /// <param name="message">The error message</param>
        public static void Error(string message)
        {
            Debug.WriteLine("[ERROR] " + message, "bugsnag");
        }
    }
}