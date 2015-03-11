using Newtonsoft.Json;

namespace Bugsnag.Payload
{
    /// <summary>
    /// Contains information about the device the app is running on
    /// </summary>
    internal class DeviceInfo
    {
        /// <summary>
        /// Gets or sets the version of operating system (optional)
        /// </summary>
        [JsonProperty("osVersion")]
        public string OSVersion { get; set; }

        /// <summary>
        /// Gets or sets the hostname of the server (optional)
        /// </summary>
        [JsonProperty("hostname")]
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the current service pack applied to the OS (optional)
        /// </summary>
        [JsonProperty("servicePack")]
        public string ServicePack { get; set; }

        /// <summary>
        /// Gets or sets the architecture of the device i.e. 32 bit or 64 bit (optional)
        /// </summary>
        [JsonProperty("osArchitecture")]
        public string OSArchitecture { get; set; }

        /// <summary>
        /// Gets or sets the number of processors the device has (optional)
        /// </summary>
        [JsonProperty("processorCount")]
        public string ProcessorCount { get; set; }

        /// <summary>
        /// Gets or sets the computer name of the device (optional)
        /// </summary>
        [JsonProperty("machineName")]
        public string MachineName { get; set; }
    }
}
