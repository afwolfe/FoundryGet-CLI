﻿using System;
using System.Threading.Tasks;
using FoundryGet.Models;
using FoundryGet.Services;
using FoundryGet.Utilities;
using Microsoft.Extensions.CommandLineUtils;

namespace FoundryGet.Commands
{
    public static class InstallCommand
    {
        public static void Command(CommandLineApplication command)
        {
            command.Description = "Install a module";
            command.HelpOption("-?|-h|--help");

            var url = command.Argument("[url]", "The manifest URL of the module");
            var dataFolder = command.Option(
                "-d|--dataFolder",
                "The Foundry Data folder to install in, such as C:\\Users\\Me\\AppData\\Local\\FoundryVTT\\Data",
                CommandOptionType.SingleValue
            );

            command.OnExecute(async () => await Execute(url, dataFolder));
        }

        private static async Task<int> Execute(CommandArgument url, CommandOption dataFolder)
        {
            try
            {
                if (string.IsNullOrEmpty(url.Value))
                {
                    Console.WriteLine("Please specify a module url to install from");
                    return 1;
                }

                FoundryDataFolder foundryDataFolder = FoundryGetUtils.GetFoundryDataFolder(
                    dataFolder
                );

                if (foundryDataFolder == null)
                    return 1;

                var manifestLoader = new ManifestLoader();
                await foundryDataFolder.ReadAllManifests(manifestLoader);

                var manifest = await manifestLoader.FromUri(new Uri(url.Value));
                if (manifest == null)
                    return 1;

                var dependencyChain = new DependencyChain();
                dependencyChain.AddCurrentlyInstalledDependencies(
                    manifestLoader,
                    foundryDataFolder
                );
                await dependencyChain.AddDependenciesFromManifest(manifestLoader, manifest);

                foreach (var dependency in dependencyChain.NeededDependencies)
                {
                    Console.WriteLine(dependency.Name);
                    var dependencyManifest = await dependency.GetFullManifest(manifestLoader);
                    await dependencyManifest.Install(foundryDataFolder);
                }

                return await manifest.Install(foundryDataFolder);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return 1;
            }
        }
    }
}
