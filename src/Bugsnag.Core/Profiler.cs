using System;
using System.Management;
using System.Net;

namespace Bugsnag.Core
{
    /// <summary>
    /// Helper class used to profile the machine / device the application is running on
    /// </summary>
    public static class Profiler
    {
        /// <summary>
        /// The version of the operating system the application is running
        /// </summary>
        public static readonly string DetectedOSVersion = GetOSInfo();

        /// <summary>
        /// The service pack installed on the operating system
        /// </summary>
        public static readonly string ServicePack = Environment.OSVersion.ServicePack;

        /// <summary>
        /// Determines if the application is a 32 bit or a 64 bit process
        /// </summary>
        public static readonly string AppArchitecture = Environment.Is64BitProcess ? "64 bit" : "32 bit";

        /// <summary>
        /// Determines if the operation system is 32 bit or 64 bit
        /// </summary>
        public static readonly string OSArchitecture = Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit";

        /// <summary>
        /// The number of processors (cores) the device/machine has
        /// </summary>
        public static readonly string ProcessorCount = Environment.ProcessorCount + " core(s)";

        /// <summary>
        /// The name of the machine the app is running on
        /// </summary>
        public static readonly string MachineName = Environment.MachineName;

        /// <summary>
        /// The Common Language Runtime (CLR) the .NET framework is using
        /// </summary>
        public static readonly string ClrVersion = Environment.Version.ToString();

        /// <summary>
        /// The hostname of the local machine
        /// </summary>
        public static readonly string HostName = Dns.GetHostName();

        /// <summary>
        /// Detects and returns the current operating system version
        /// </summary>
        /// <returns>The operating system version</returns>
        private static string GetOSInfo()
        {
            switch (Environment.OSVersion.Platform)
            {
                // Platform is Windows 95, Windows 98, Windows 98 Second Edition,
                // or Windows Me.
                case System.PlatformID.Win32Windows:
                    return GetWin32WindowsVersion();
                case System.PlatformID.Win32NT:
                    return GetWin32NTVersion();
                default:
                    return "UNKNOWN";
            }
        }

        /// <summary>
        /// Detects the current operating system version if its Win32 Windows
        /// </summary>
        /// <returns>The operation system version</returns>
        private static string GetWin32WindowsVersion()
        {
            // Platform is Windows 95, Windows 98, Windows 98 Second Edition,
            // or Windows Me.
            switch (Environment.OSVersion.Version.Minor)
            {
                case 0:
                    return "Windows 95";
                case 10:
                    return "Windows 98";
                case 90:
                    return "Windows Me";
                default:
                    return "UNKNOWN";
            }
        }

        /// <summary>
        /// Detects the current operating system version if its Win32 NT
        /// </summary>
        /// <returns>The operation system version</returns>
        private static string GetWin32NTVersion()
        {
            switch (Environment.OSVersion.Version.Major)
            {
                case 3:
                    return "Windows NT 3.51";
                case 4:
                    return "Windows NT 4.0";
                case 5:
                    return Environment.OSVersion.Version.Minor == 0 ? "Windows 2000" : "Windows XP";
                case 6:
                    if (Environment.OSVersion.Version.Minor == 0)
                        return "Windows Server 2008";

                    if (Environment.OSVersion.Version.Minor == 1)
                        return IsServerVersion() ? "Windows Server 2008 R2" : "Windows 7";

                    if (Environment.OSVersion.Version.Minor == 2)
                        return IsServerVersion() ? "Windows Server 2012" : "Windows 8";

                    if (Environment.OSVersion.Version.Minor == 3)
                        return IsServerVersion() ? "Windows Server 2012 R2" : "Windows 8.1";

                    return "UNKNOWN";
                default:
                    return "UNKNOWN";
            }
        }

        /// <summary>
        /// Determines if the current operating system is the server version 
        /// </summary>
        /// <returns>True if the current operating system is the server version, otherwise false</returns>
        private static bool IsServerVersion()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                foreach (var managementObject in searcher.Get())
                {
                    // ProductType will be one of:
                    // 1: Workstation
                    // 2: Domain Controller
                    // 3: Server
                    uint productType = (uint)managementObject.GetPropertyValue("ProductType");
                    return productType != 1;
                }
            }
            return false;
        }
    }
}
