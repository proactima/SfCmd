using Microsoft.Extensions.CommandLineUtils;

namespace ServiceFabricUploader.Commands.ApplicationType
{
    public class ApplicationTypeCommands
    {
        public static void Configure(CommandLineApplication app)
        {
            app.Command("applicationtype", application =>
            {
                ProvisionCommand.Configure(application);
            });
        }
    }
}