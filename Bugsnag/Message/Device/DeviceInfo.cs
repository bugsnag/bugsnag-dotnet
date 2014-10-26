using Newtonsoft.Json;

namespace Bugsnag.Message.Device
{
    public class DeviceInfo
    {
        [JsonProperty("osVersion")]
        public string OsVersion { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("servicePack")]
        public string ServicePack { get; set; }
    }
}
