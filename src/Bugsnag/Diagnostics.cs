using System;
using System.Net;

#if !MONO
using System.Management;
#endif

namespace Bugsnag
{
    /// <summary>
    /// Helper class used to profile the machine / device the application is running on
    /// </summary>
    internal static class Diagnostics
    {
        /// <summary>
        /// Enum holding the different types of OS
        /// </summary>
        public enum OsType
        {
            Server,
            Desktop,
            Unknown
        }

        /// <summary>
        /// The version of the operating system the application is running
        /// </summary>
        public static readonly string DetectedOSVersion = GetOSInfo();

        /// <summary>
        /// The service pack installed on the operating system
        /// </summary>
        public static readonly string ServicePack = Environment.OSVersion.ServicePack == "" ? null : Environment.OSVersion.ServicePack;

        /// <summary>
        /// Determines if the application is a 32 bit or a 64 bit process
        /// </summary>
        public static readonly string AppArchitecture = Is64bitProcess() ? "64 bit" : "32 bit";

        /// <summary>
        /// Determines if the operation system is 32 bit or 64 bit
        /// </summary>
        public static readonly string OSArchitecture = GetOSArchitecture();

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
                    return UnixOrMacVersion();
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
                    {
                        switch (GetOsType())
                        {
                            case OsType.Desktop:
                                return "Windows 7";
                            case OsType.Server:
                                return "Windows Server 2008 R2";
                            case OsType.Unknown:
                                return "Windows 7 / Server 2008 R2";
                        }
                    }

                    if (Environment.OSVersion.Version.Minor == 2)
                    {
                        switch (GetOsType())
                        {
                            case OsType.Desktop:
                                return "Windows 8";
                            case OsType.Server:
                                return "Windows Server 2012";
                            case OsType.Unknown:
                                return "Windows 8 / Server 2012";
                        }
                    }


                    if (Environment.OSVersion.Version.Minor == 3)
                    {
                        switch (GetOsType())
                        {
                            case OsType.Desktop:
                                return "Windows 8.1";
                            case OsType.Server:
                                return "Windows Server 2012 R2";
                            case OsType.Unknown:
                                return "Windows 8 / Server 2012 R2";
                        }
                    }
                    return "UNKNOWN";
                default:
                    return "UNKNOWN";
            }
        }

        /// <summary>
        /// Determines if the current operating system is the server version 
        /// </summary>
        /// <returns>True if the current operating system is the server version, otherwise false</returns>
        private static OsType GetOsType()
        {
            #if !MONO
            try
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
                        return (productType != 1 ? OsType.Server : OsType.Desktop);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // If we don't have permssions to query the WMI, then indicate we don't know the OS type
                return OsType.Unknown;
            }
            #endif
            return OsType.Desktop;
        }

        /// <summary>
        /// Determines the OS version if on a UNIX based system
        /// </summary>
        /// <returns></returns>
        private static string UnixOrMacVersion()
        {
            #if UNIFIED 
            return Foundation.NSProcessInfo.ProcessInfo.OperatingSystemVersionString;
            #elif ANDROID
            return "Android SDK " + Android.OS.Build.VERSION.SdkInt;
            #else

            if (RunTerminalCommand("uname") == "Darwin")
            {
                var osName = RunTerminalCommand("sw_vers", "-productName");
                var osVersion = RunTerminalCommand("sw_vers", "-productVersion");
                return osName + " (" + osVersion + ")";
            }
            return "UNIX";
            #endif
        }

        /// <summary>
        /// Executes a command with arguments, used to send terminal commands in UNIX systems
        /// </summary>
        /// <param name="cmd">The command to send</param>
        /// <param name="args">The arguments to send</param>
        /// <returns>The returned output</returns>
        private static string RunTerminalCommand(string cmd, string args = null)
        {
            var proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = cmd;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.WaitForExit();
            var output = proc.StandardOutput.ReadToEnd();
            return output.Trim();
        }

        /// <summary>
        /// Determines if the current process is a 64 bit process
        /// </summary>
        /// <returns>True if the process is 64 bit otherwise false</returns>
        private static bool Is64bitProcess()
        {
#if !NET35
            return Environment.Is64BitProcess;
#else
            return IntPtr.Size == 8;
#endif
        }

        /// <summary>
        /// Attempts to determine the architecture of the OS.
        /// </summary>
        /// <returns>The architecture of the OS</returns>
        private static string GetOSArchitecture()
        {
#if !NET35
            return Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit";
#else
            return Is64bitProcess() ? "64 bit" : "Unknown";
#endif
        }
    }
}
