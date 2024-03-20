using System;
using System.Threading.Tasks;
using FoundryGet.Models;

namespace FoundryGet.Interfaces
{
    public interface IManifestLoader
    {
        Task<Manifest> FromUri(Uri uri);

        Task<Manifest> FromFile(string filePath);
    }
}
