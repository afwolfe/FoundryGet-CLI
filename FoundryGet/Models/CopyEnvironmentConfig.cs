using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FoundryGet.Services;
using Newtonsoft.Json;

namespace FoundryGet.Models
{
    public partial class CopyEnvironmentConfig
    {
        [JsonProperty("core.version")]
        public string CoreVersion { get; set; }

        [JsonProperty("system.manifest")]
        public string SystemManifest { get; set; }

        [JsonProperty("system.version")]
        public string SystemVersion { get; set; }

        [JsonProperty("modules", NullValueHandling = NullValueHandling.Ignore)]
        public List<Dependency> Modules { get; set; } = new List<Dependency>();
    }

    public partial class CopyEnvironmentConfig
    {
        private HttpClient _httpClient = new HttpClient();

        public Task<int[]> InstallAll(FoundryDataFolder dataFolder)
        {
            var manifestLoader = new ManifestLoader();
            return Task.WhenAll(
                Modules.Select(async module =>
                {
                    var moduleManifest = await module.GetFullManifest(manifestLoader);
                    return await moduleManifest.InstallExactVersion(dataFolder, module.RawVersion);
                })
            );
        }
    }
}
