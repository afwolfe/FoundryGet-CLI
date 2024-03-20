﻿using System;
using System.Diagnostics;
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

        [JsonProperty("version")]
        [JsonConverter(typeof(SemanticVersionConverter))]
        public SemanticVersioning.Version Version { get; set; }

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
    }
}
