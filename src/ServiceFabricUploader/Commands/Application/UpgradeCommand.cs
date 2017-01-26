using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using ServiceFabricUploader.Models;

namespace ServiceFabricUploader.Commands.Application
{
    public class UpgradeCommand
    {
        private static AppOptionsRaw _appOptions;
        private static UpgradeCommandOptionsRaw _upgradeCommandOptions;

        public static void Configure(CommandLineApplication app)
        {
            app.Command("upgrade", application =>
            {
                _appOptions = new AppOptionsRaw(application);
                _upgradeCommandOptions = new UpgradeCommandOptionsRaw(application);

                application.OnExecute(async () =>
                {
                    var appConfig = AppOptions.ValidateAndCreate(_appOptions);
                    var commandConfig = UpgradeCommandOptions.VerifyAndCreateArgs(_upgradeCommandOptions);
                    var command = new UpgradeCommand();
                    return await command.RunAsync(appConfig, commandConfig).ConfigureAwait(false);
                });
            });
        }

        public async Task<int> RunAsync(AppOptions appOptions, UpgradeCommandOptions commandConfig)
        {
            var connectionInfo = ConnectionInfoHelper.CreateConnectionInfo(appOptions);
            var connection = connectionInfo.CreateClusterConnection();
            var logger = new Logger(appOptions.Verbose);

            var register = new SfRestApi.Endpoints.Application(connection, logger);
            var upgradeOptions = commandConfig.ToApplicationModel();
            var registerSuccess = await register.UpgradeAsync(upgradeOptions).ConfigureAwait(false);

            return registerSuccess ? 0 : -1;
        }
    }
}