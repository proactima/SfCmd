using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace ServiceFabricUploader.Commands.ImageStore
{
    public class UploadCommandOptionsRaw
    {
        public UploadCommandOptionsRaw(CommandLineApplication app)
        {
            PackageSourcePath = app.Option("--packageSource", "Source path of the package",
                CommandOptionType.SingleValue);
            PackageName = app.Option("--packageName", "Package name in ImageStore",
                CommandOptionType.SingleValue);
        }

        public CommandOption PackageSourcePath { get; set; }
        public CommandOption PackageName { get; set; }
    }

    public class UploadCommandOptions
    {
        public DirectoryInfo PackageSourcePath { get; set; }
        public string PackageName { get; set; }
        
        public static UploadCommandOptions VerifyAndCreateArgs(UploadCommandOptionsRaw rawConfig)
        {
            if (!rawConfig.PackageSourcePath.HasValue())
                throw new ArgumentException("No Package Source Path specified");

            if (!rawConfig.PackageName.HasValue())
                throw new ArgumentException("No Package Name specified");

            return new UploadCommandOptions
            {
                PackageSourcePath = new DirectoryInfo(rawConfig.PackageSourcePath.Value()),
                PackageName = rawConfig.PackageName.Value()
            };
        }
    }
}