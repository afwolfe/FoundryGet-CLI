using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FoundryGet.Interfaces;
using Newtonsoft.Json;

namespace FoundryGet.Models
{
    [DebuggerDisplay("{Name} {Version}")]
    public class Dependency
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("manifest")]
        public Uri ManifestUri { get; set; }

        SemanticVersioning.Version _version;

        string _rawVersion;

        [JsonProperty("version")]
        public string RawVersion
        {
            get { return _rawVersion; }
            set
            {
                _rawVersion = value;
                try
                {
                    _version = new SemanticVersioning.Version(value);
                }
                catch
                {
                    Console.WriteLine($"Error setting semver for {Name} to: {value}");
                    _version = new SemanticVersioning.Version("0.0.0");
                }
            }
        }

        public SemanticVersioning.Version Version
        {
            get { return _version; }
        }

        public bool IsSatisfiedBy(Manifest manifest)
        {
            return IsSatisfiedBy(manifest.ToDependency());
        }

        public bool IsSatisfiedBy(Dependency dependency)
        {
            return Name == dependency.Name
                && Version.Major == dependency.Version.Major
                && Version.Minor <= dependency.Version.Minor;
        }

        public async Task<Manifest> GetFullManifest(IManifestLoader manifestLoader)
        {
            return await manifestLoader.FromUri(ManifestUri);
        }

        public async Task<Manifest> GetFullManifestAtVersion(IManifestLoader manifestLoader)
        {
            ManifestUri = new Uri(
                Regex.Replace(
                    ManifestUri.AbsoluteUri,
                    "/latest/download",
                    $"/download/{RawVersion}"
                )
            );
            return await manifestLoader.FromUri(ManifestUri);
        }
    }
}
