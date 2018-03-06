using System.Diagnostics;

namespace Bugsnag
{
    /// <summary>
    /// Default logging to use in this Bugsnag library
    /// </summary>
    internal static class Logger
    {
        /// <remarks>
        /// The EventLog and EventLog.Source are usually different, but specifying a custom source (like "Bugsnag")
        /// requires administrative priviledges to setup before it can be used. Using an existing source --
        /// like "Application", which should always already exist -- side steps the issue. See
        /// http://stackoverflow.com/a/27640623/1462295
        /// </remarks>
        private const string EventLogSource = "Application";

        /// <summary>
        /// Record an informational message
        /// </summary>
        /// <param name="message">The info message</param>
        public static void Info(string message)
        {
            Debug.WriteLine("[INFO] " + message, "bugsnag");

            EventLogMessage(message, EventLogEntryType.Information);
        }

        /// <summary>
        /// Record a warning message
        /// </summary>
        /// <param name="message">The warning message</param>
        public static void Warning(string message)
        {
            Debug.WriteLine("[WARN] " + message, "bugsnag");

            EventLogMessage(message, EventLogEntryType.Warning);
        }

        /// <summary>
        /// Record an error message
        /// </summary>
        /// <param name="message">The error message</param>
        public static void Error(string message)
        {
            Debug.WriteLine("[ERROR] " + message, "bugsnag");

            EventLogMessage(message, EventLogEntryType.Error);
        }

        /// <summary>
        /// Helper method to write unhandled Bugsnag error to Windows event log.
        /// </summary>
        /// <param name="message">Message to write to the event log.</param>
        /// <param name="entryType">Severity of message to write.</param>
        private static void EventLogMessage(string message, EventLogEntryType entryType)
        {
            try
            {
                using (EventLog eventLog = new EventLog(EventLogSource))
                {
                    // Source name may be truncated at 21 characters, or may possibly fail if longer than 21 characters.
                    // See comments at https://msdn.microsoft.com/en-us/library/windows/desktop/aa363661%28v=vs.85%29.aspx
                    eventLog.Source = EventLogSource;
                    eventLog.WriteEntry(message, entryType);
                }
            }
            catch
            {
                Debug.WriteLine("[ERROR] Could not write to EventLog.", "bugsnag");
            }
        }
    }
}