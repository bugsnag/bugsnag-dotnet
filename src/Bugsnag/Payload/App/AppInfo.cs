using Newtonsoft.Json;

namespace Bugsnag.Payload
{
    /// <summary>
    /// Contains information about the app that crashed
    /// </summary>
    internal class AppInfo
    {
        /// <summary>
        /// Gets or sets the version number of the application (optional)
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the release stage of the application e.g. development, production (optional)
        /// </summary>
        [JsonProperty("releaseStage")]
        public string ReleaseStage { get; set; }

        /// <summary>
        /// Gets or sets the architecture of the application i.e. 32 bit or 64 bit application (optional)
        /// </summary>
        [JsonProperty("appArchitecture")]
        public string AppArchitecture { get; set; }

        /// <summary>
        /// Gets or sets the version of the Common Language Runtime (CLR) the .NET framework is using (optional)
        /// </summary>
        [JsonProperty("clrVersion")]
        public string ClrVersion { get; set; }
    }
}