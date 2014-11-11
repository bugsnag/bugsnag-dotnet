using System;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bugsnag.Payload.Device;

namespace Bugsnag
{
    public static class Profiler
    {
        public static readonly string DetectedOsVersion;
        public static readonly string ServicePack;
        public static readonly string AppArchitecture;
        public static readonly string OsArchitecture;
        public static readonly string ProcessorCount;
        public static readonly string MachineName;
        public static readonly string ClrVersion;
 
        static Profiler()
        {
            DetectedOsVersion = GetOsInfo();
            ServicePack = Environment.OSVersion.ServicePack;
            AppArchitecture = Environment.Is64BitProcess ? "64 bit" : "32 bit";
            OsArchitecture = Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit";
            ProcessorCount = Environment.ProcessorCount + " cores";
            MachineName = Environment.MachineName;
            ClrVersion = Environment.Version.ToString();
        }

        private static string GetOsInfo()
        {
            var osInfo = Environment.OSVersion;

            switch (osInfo.Platform)
            {
                // Platform is Windows 95, Windows 98, Windows 98 Second Edition,
                // or Windows Me.
                case System.PlatformID.Win32Windows:
                    return GetWin32WindowsVersion(osInfo);
                case System.PlatformID.Win32NT:
                    return GetWin32NTVersion(osInfo);
                default:
                    return "UNKNOWN";
            }
        }

        private static string GetWin32WindowsVersion(OperatingSystem osInfo)
        {
            // Platform is Windows 95, Windows 98, Windows 98 Second Edition,
            // or Windows Me.
            switch (osInfo.Version.Minor)
            {
                case 0:
                    return "Windows 95";
                case 10:
                    return osInfo.Version.Revision.ToString() == "2222A"? "Windows 98 Second Edition" : "Windows 98";
                case 90:
                    return "Windows Me";
                default:
                    return "UNKNOWN";
             }
        }

        private static string GetWin32NTVersion(OperatingSystem osInfo)
        {
            switch (osInfo.Version.Major)
            {
                case 3:
                    return "Windows NT 3.51";
                case 4:
                    return "Windows NT 4.0";
                case 5:
                    return osInfo.Version.Minor == 0 ? "Windows 2000" : "Windows XP";
                case 6:
                    if (osInfo.Version.Minor == 0)
                        return "Windows Server 2008";

                    if (osInfo.Version.Minor == 1)
                        return IsServerVersion() ? "Windows Server 2008 R2" : "Windows 7";

                    if (osInfo.Version.Minor == 2)
                        return IsServerVersion() ? "Windows Server 2012" : "Windows 8";
                    
                    if (osInfo.Version.Minor == 3)
                        return IsServerVersion() ? "Windows Server 2012 R2" : "Windows 8.1";

                    return "UNKNOWN";
                default:
                    return "UNKNOWN";
            }
        }

        private static bool IsServerVersion()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                foreach (ManagementObject managementObject in searcher.Get())
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
