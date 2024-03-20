using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FoundryGet.Models;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace FoundryGet.Commands
{
    public static class SyncCommand
    {
        public static void Command(CommandLineApplication command)
        {
            command.Description = "Sync with an environment config";
            command.HelpOption("-?|-h|--help");

            var configPath = command.Argument(
                "[configPath]",
                "The local path of the environment config json"
            );
            var dataFolder = command.Option(
                "-d|--dataFolder",
                "The Foundry Data folder to install in, such as C:\\Users\\Me\\AppData\\Local\\FoundryVTT\\Data",
                CommandOptionType.SingleValue
            );

            var generateLockFile = command.Option(
                "-l|--lock",
                "Generate a lock file from the environment, with absolute download URLs",
                CommandOptionType.NoValue
            );

            command.OnExecute(async () => await Execute(configPath, dataFolder, generateLockFile));
        }

        private static async Task<int> Execute(
            CommandArgument configPath,
            CommandOption dataFolder,
            CommandOption generateLockfile
        )
        {
            try
            {
                if (string.IsNullOrEmpty(configPath.Value))
                {
                    Console.WriteLine("Please specify an environment config to sync from");
                    return 1;
                }

                FoundryDataFolder foundryDataFolder = FoundryDataFolder.GetFoundryDataFolder(
                    dataFolder
                );
                if (foundryDataFolder == null)
                {
                    return 1;
                }

                var json = await File.ReadAllTextAsync(configPath.Value);
                var copyEnvironmentConfig = JsonConvert.DeserializeObject<CopyEnvironmentConfig>(
                    json
                );

                var results = await copyEnvironmentConfig.InstallAll(
                    foundryDataFolder,
                    generateLockfile.HasValue()
                );
                if (results.Contains(1))
                {
                    return 1;
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return 1;
            }
        }
    }
}
