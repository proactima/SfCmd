﻿using System;
using Microsoft.Extensions.CommandLineUtils;
using ServiceFabricUploader.Commands.Application;
using ServiceFabricUploader.Commands.ApplicationType;
using ServiceFabricUploader.Commands.ImageStore;

namespace ServiceFabricUploader
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var app = new CommandLineApplication
                {
                    Name = "SFTools",
                };
                app.HelpOption("-?|-h|--help");

                ImageStoreCommand.Configure(app);
                ApplicationTypeCommands.Configure(app);
                ApplicationCommands.Configure(app);

                var errCode = app.Execute(args);
                if (errCode != 0)
                {
                    //Console.WriteLine("Error parsing options...");
                    //Console.WriteLine();

                    //app.ShowHelp();
                    Console.WriteLine($"Something went wrong - exited with status code {errCode}");
                    return errCode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

            return 0;
        }
    }
}