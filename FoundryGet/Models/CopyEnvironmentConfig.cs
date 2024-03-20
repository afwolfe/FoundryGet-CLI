using System;
using System.Collections.Generic;
using System.IO;
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

        private static void writeUrisToLockfile(List<string> uris, string lockfilePath)
        {
            if (uris == null || uris.Count == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(lockfilePath))
            {
                throw new ArgumentException("File path cannot be null or empty.");
            }

            uris.Sort();

            File.WriteAllLines(lockfilePath, uris);
        }

        public async Task<int[]> InstallAll(FoundryDataFolder dataFolder, bool generateLockfile)
        {
            var manifestLoader = new ManifestLoader();
            var installedUris = new List<string>();

            var results = await Task.WhenAll(
                Modules.Select(async module =>
                {
                    var moduleManifest = await module.GetFullManifest(manifestLoader);
                    return await moduleManifest.InstallExactVersion(
                        dataFolder,
                        module.RawVersion,
                        installedUris
                    );
                })
            );
            if (generateLockfile)
            {
                writeUrisToLockfile(installedUris, "./lockfile.txt");
            }
            return results;
        }
    }
}
