using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FoundryGet.Interfaces;
using FoundryGet.Models;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace FoundryGet.Utilities
{
    public class FoundryGetUtils
    {
        public static FoundryDataFolder GetFoundryDataFolder(CommandOption dataFolder)
        {
            if (dataFolder.HasValue())
            {
                return FoundryDataFolder.FromDirectoryPath(dataFolder.Value());
            }
            else
            {
                return FoundryDataFolder.FromCurrentDirectory();
            }

            throw new FileNotFoundException("Unable to determine FoundryDataFolder");
        }
    }
}
