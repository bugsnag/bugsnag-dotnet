using Newtonsoft.Json;

namespace Bugsnag.Core.Payload
{
    public class DeviceInfo
    {
        [JsonProperty("osVersion")]
        public string OSVersion { get; set; }

        [JsonProperty("hostname")]
        public string HostName { get; set; }

        [JsonProperty("servicePack")]
        public string ServicePack { get; set; }

        [JsonProperty("osArchitecture")]
        public string OSArchitecture { get; set; }

        [JsonProperty("processorCount")]
        public string ProcessorCount { get; set; }

        [JsonProperty("machineName")]
        public string MachineName { get; set; }
    }
}
