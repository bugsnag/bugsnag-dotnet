using Newtonsoft.Json;

namespace Bugsnag.Payload.Device
{
    public class DeviceInfo
    {
        [JsonProperty("osVersion")]
        public string OsVersion { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("servicePack")]
        public string ServicePack { get; set; }

        [JsonProperty("osArchitecture")]
        public string OsArchitecture { get; set; }

        [JsonProperty("processorCount")]
        public string ProcessorCount { get; set; }

        [JsonProperty("machineName")]
        public string MachineName { get; set; }
    }
}
