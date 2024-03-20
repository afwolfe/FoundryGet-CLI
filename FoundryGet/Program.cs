﻿using System;
using FoundryGet.Commands;
using Microsoft.Extensions.CommandLineUtils;

namespace FoundryGet
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication { Name = "foundryget" };

            app.HelpOption("-?|-h|--help");

            app.OnExecute(() =>
            {
                Console.WriteLine("Need help? Run `foundryget -?`");
                return 0;
            });

            app.Command("install", InstallCommand.Command);
            app.Command("update", UpdateCommand.Command);
            app.Command("sync", SyncCommand.Command);

            app.Execute(args);
        }
    }
}
