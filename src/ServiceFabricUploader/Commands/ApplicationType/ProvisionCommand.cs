using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using ServiceFabricUploader.Models;
using SfRestApi.Endpoints;

namespace ServiceFabricUploader.Commands.ApplicationType
{
    public class ProvisionCommand
    {
        private static AppOptionsRaw _appOptions;
        private static ProvisionCommandOptionsRaw _provisionCommandOptions;

        public static void Configure(CommandLineApplication app)
        {
            app.Command("provision", application =>
            {
                _appOptions = new AppOptionsRaw(application);
                _provisionCommandOptions = new ProvisionCommandOptionsRaw(application);

                application.OnExecute(async () =>
                {
                    var appConfig = AppOptions.ValidateAndCreate(_appOptions);
                    var commandConfig = ProvisionCommandOptions.VerifyAndCreateArgs(_provisionCommandOptions);
                    var command = new ProvisionCommand();
                    return await command.RunAsync(appConfig, commandConfig).ConfigureAwait(false);
                });
            });
        }

        public async Task<int> RunAsync(AppOptions appOptions, ProvisionCommandOptions commandConfig)
        {
            var connectionInfo = ConnectionInfoHelper.CreateConnectionInfo(appOptions);
            var connection = connectionInfo.CreateClusterConnection();
            var logger = new Logger(appOptions.Verbose);

            var register = new Register(connection, logger);
            var registerSuccess = await register.RegisterAsync(commandConfig.PackageName).ConfigureAwait(false);

            return registerSuccess ? 0 : -1;
        }
    }
}