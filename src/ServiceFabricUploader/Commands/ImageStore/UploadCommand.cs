using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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
                    commandConfig.PackageSourcePath.FullName)
                .Select(x => new Tuple<FileInfo, string>(x.Key, x.Value));

            var logger = new Logger(appOptions.Verbose);
            var imageStore = new SfRestApi.Endpoints.ImageStore(connection, logger);
            var failedUploads = new ConcurrentBag<FileInfo>();

            var uploadFileBlock = new TransformBlock<Tuple<FileInfo, string>, FileInfo>(async tuple =>
            {
                var success = await imageStore
                    .UploadAsync(tuple.Item1, tuple.Item2, commandConfig.PackageName)
                    .ConfigureAwait(false);

                return success
                    ? null
                    : tuple.Item1;
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 4
            });

            var handleFailuresBlock = new ActionBlock<FileInfo>(file =>
            {
                if (file != null)
                    failedUploads.Add(file);
            });

            uploadFileBlock.LinkTo(handleFailuresBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            foreach (var file in filesToUpload)
                uploadFileBlock.Post(file);

            uploadFileBlock.Complete();
            handleFailuresBlock.Completion.Wait();

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