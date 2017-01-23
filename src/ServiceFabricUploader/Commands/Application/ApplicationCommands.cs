using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using ServiceFabricUploader.Models;

namespace ServiceFabricUploader.Commands.Application
{
    public class ApplicationCommands
    {
        public static void Configure(CommandLineApplication app)
        {
            app.Command("application", application =>
            {
                ProvisionCommand.Configure(application);
            });
        }
    }
}