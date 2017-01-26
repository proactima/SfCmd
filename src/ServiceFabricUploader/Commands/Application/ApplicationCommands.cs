using Microsoft.Extensions.CommandLineUtils;

namespace ServiceFabricUploader.Commands.Application
{
    public class ApplicationCommands
    {
        public static void Configure(CommandLineApplication app)
        {
            app.Command("application", application =>
            {
                UpgradeCommand.Configure(application);
            });
        }
    }
}