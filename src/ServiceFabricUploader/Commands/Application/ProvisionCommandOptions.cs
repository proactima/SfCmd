using System;
using Microsoft.Extensions.CommandLineUtils;

namespace ServiceFabricUploader.Commands.Application
{
    public class ProvisionCommandOptionsRaw
    {
        public ProvisionCommandOptionsRaw(CommandLineApplication app)
        {
            PackageName = app.Option("--packageName", "Package name in ImageStore", CommandOptionType.SingleValue);
        }

        public CommandOption PackageName { get; set; }
    }

    public class ProvisionCommandOptions
    {
        internal ProvisionCommandOptions(ProvisionCommandOptionsRaw rawConfig)
        {
            if (!rawConfig.PackageName.HasValue())
                throw new ArgumentException("No Package Name specified");

            PackageName = rawConfig.PackageName.Value();
        }

        public string PackageName { get; set; }

        public static ProvisionCommandOptions VerifyAndCreateArgs(ProvisionCommandOptionsRaw rawConfig)
        {
            return new ProvisionCommandOptions(rawConfig);
        }
    }
}