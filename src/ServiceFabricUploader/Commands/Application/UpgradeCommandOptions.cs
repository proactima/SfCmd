using System;
using Microsoft.Extensions.CommandLineUtils;
using SfRestApi;
using SfRestApi.Models.Endpoints;

namespace ServiceFabricUploader.Commands.Application
{
    public class UpgradeCommandOptionsRaw
    {
        public UpgradeCommandOptionsRaw(CommandLineApplication app)
        {
            ApplicationName = app.Option("--applicationName", "Application Name", CommandOptionType.SingleValue);
            TargetVersion = app.Option("--targetVersion", "Target Version", CommandOptionType.SingleValue);
            RollingUpgradeMode = app.Option("--upgradeMode", "Upgrade Mode - Defaults to Monitored",
                CommandOptionType.SingleValue);
            ForceRestart = app.Option("--forceRestart", "Force Restart", CommandOptionType.NoValue);
            FailureAction = app.Option("--failureAction", "Failure Action - Defaults to Rollback",
                CommandOptionType.SingleValue);
            HealthCheckWaitDuration = app.Option("--healthCheckWaitDuration", "Defaults to 60 seconds",
                CommandOptionType.SingleValue);
            HealthCheckStableDuration = app.Option("--healthCheckStableDuration", "Defaults to 60 seconds",
                CommandOptionType.SingleValue);
            HealthCheckRetryTimeout = app.Option("--healthCheckRetryTimeout", "Defaults to 60 seconds",
                CommandOptionType.SingleValue);
        }

        public CommandOption ApplicationName { get; set; }
        public CommandOption TargetVersion { get; set; }
        public CommandOption RollingUpgradeMode { get; set; }
        public CommandOption ForceRestart { get; set; }
        public CommandOption FailureAction { get; set; }
        public CommandOption HealthCheckWaitDuration { get; set; }
        public CommandOption HealthCheckStableDuration { get; set; }
        public CommandOption HealthCheckRetryTimeout { get; set; }
    }

    public class UpgradeCommandOptions
    {
        internal UpgradeCommandOptions(UpgradeCommandOptionsRaw rawConfig)
        {
            if (!rawConfig.ApplicationName.HasValue())
                throw new ArgumentException("No Application Name specified");
            if (!rawConfig.TargetVersion.HasValue())
                throw new ArgumentException("No Target Version specified");

            ApplicationName = rawConfig.ApplicationName.Value();
            TargetVersion = rawConfig.TargetVersion.Value();
            RollingUpgradeMode = OptionsHelper.GetEnumValueOrDefault(rawConfig.RollingUpgradeMode,
                Constants.RollingUpgradeMode.Monitored);
            ForceRestart = rawConfig.ForceRestart.HasValue();
            FailureAction = OptionsHelper.GetEnumValueOrDefault(rawConfig.FailureAction,
                Constants.FailureAction.Rollback);
            HealthCheckWaitDuration = OptionsHelper.GetIntOrDefaultValue(rawConfig.HealthCheckWaitDuration, 60);
            HealthCheckStableDuration = OptionsHelper.GetIntOrDefaultValue(rawConfig.HealthCheckStableDuration, 60);
            HealthCheckRetryTimeout = OptionsHelper.GetIntOrDefaultValue(rawConfig.HealthCheckRetryTimeout, 60);
        }

        public string ApplicationName { get; set; }
        public string TargetVersion { get; set; }
        public Constants.RollingUpgradeMode RollingUpgradeMode { get; set; }
        public bool ForceRestart { get; set; }
        public Constants.FailureAction FailureAction { get; set; }
        public int HealthCheckWaitDuration { get; set; }
        public int HealthCheckStableDuration { get; set; }
        public int HealthCheckRetryTimeout { get; set; }

        public static UpgradeCommandOptions VerifyAndCreateArgs(UpgradeCommandOptionsRaw rawConfig)
        {
            return new UpgradeCommandOptions(rawConfig);
        }
    }

    public static class UpgradeCommandOptionHelpers
    {
        public static ApplicationUpgrade ToApplicationModel(this UpgradeCommandOptions input)
        {
            return new ApplicationUpgrade
            {
                ApplicationName = input.ApplicationName,
                TargetVersion = input.TargetVersion,
                FailureAction = input.FailureAction,
                ForceRestart = input.ForceRestart,
                HealthCheckWaitDuration = input.HealthCheckWaitDuration,
                HealthCheckRetryTimeout = input.HealthCheckRetryTimeout,
                HealthCheckStableDuration = input.HealthCheckStableDuration,
                RollingUpgradeMode = input.RollingUpgradeMode
            };
        }
    }
}