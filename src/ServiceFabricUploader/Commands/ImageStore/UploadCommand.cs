using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using ServiceFabricUploader.Models;

namespace ServiceFabricUploader.Commands.ImageStore
{
    public class UploadCommand
    {
        private static AppOptionsRaw _appOptions;
        private static UploadCommandOptionsRaw _uploadCommandOptions;

        public static void Configure(CommandLineApplication app)
        {
            app.Command("upload", application =>
            {
                _appOptions = new AppOptionsRaw(application);
                _uploadCommandOptions = new UploadCommandOptionsRaw(application);

                application.OnExecute(async () =>
                {
                    var appConfig = AppOptions.ValidateAndCreate(_appOptions);
                    var commandConfig = UploadCommandOptions.VerifyAndCreateArgs(_uploadCommandOptions);
                    var command = new UploadCommand();
                    var exitCode = await command.RunAsync(appConfig, commandConfig).ConfigureAwait(false);
                    return exitCode;
                });
            });
        }
        
        public async Task<int> RunAsync(AppOptions appOptions, UploadCommandOptions commandConfig)
        {
            var connectionInfo = ConnectionInfoHelper.CreateConnectionInfo(appOptions);
            var connection = connectionInfo.CreateClusterConnection();

            var filesToUpload = DiscoverFilesToUploadRecursivly(commandConfig.PackageSourcePath,
                commandConfig.PackageSourcePath.FullName);

            var logger = new Logger(appOptions.Verbose);
            var imageStore = new SfRestApi.Endpoints.ImageStore(connection, logger);
            var failedUploads = new List<FileInfo>();

            foreach (var fileToUpload in filesToUpload)
            {
                var success =
                    await imageStore.UploadAsync(fileToUpload.Key, fileToUpload.Value, commandConfig.PackageName).ConfigureAwait(false);
                if (!success)
                    failedUploads.Add(fileToUpload.Key);
            }

            if (!failedUploads.Any())
            {
                logger.Log($"Upload completed.");
                return 0;
            }

            logger.LogError("Failed to upload some files, aborting");
            return -1;
        }

        private static Dictionary<FileInfo, string> DiscoverFilesToUploadRecursivly(DirectoryInfo sourceDirectory,
            string baseDirectory)
        {
            var filesToUpload = sourceDirectory
                .GetFiles()
                .ToDictionary(fileInfo => fileInfo,
                    fileInfo => fileInfo.FullName.Substring(baseDirectory.Length + 1).Replace("/", "\\"));

            var markFileName = Path.Combine(sourceDirectory.FullName, "_.dir");
            //File.Create(markFileName).Dispose();
            var markFile = new FileInfo(markFileName);
            filesToUpload.Add(markFile, markFile.FullName.Substring(baseDirectory.Length + 1));

            var subFolders = sourceDirectory.GetDirectories();
            if (!subFolders.Any())
                return filesToUpload;

            foreach (var subFolder in subFolders)
            {
                var fromSubFolders = DiscoverFilesToUploadRecursivly(subFolder, baseDirectory);
                foreach (var fromSubFolder in fromSubFolders)
                {
                    filesToUpload.Add(fromSubFolder.Key, fromSubFolder.Value);
                }
            }

            return filesToUpload;
        }
    }
}