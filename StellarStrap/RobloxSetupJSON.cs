using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StellarStrap
{
    public class RobloxWindowsPlayerManifest
    {
        [JsonPropertyName("version")]
        public string Version { get; }

        [JsonPropertyName("clientVersionUpload")]
        public string ClientVersionUpload { get; }

        [JsonPropertyName("bootstrapperVersion")]
        public string BootstrapperVersion { get; }
    }
}
