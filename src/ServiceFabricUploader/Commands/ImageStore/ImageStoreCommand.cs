using Microsoft.Extensions.CommandLineUtils;

namespace ServiceFabricUploader.Commands.ImageStore
{
    public class ImageStoreCommand
    {
        public static void Configure(CommandLineApplication app)
        {
            app.Command("imagestore", application =>
            {
                UploadCommand.Configure(application);
            });
        }
    }
}