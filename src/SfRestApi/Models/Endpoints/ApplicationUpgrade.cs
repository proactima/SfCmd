namespace SfRestApi.Models.Endpoints
{
    public class ApplicationUpgrade
    {
        public string ApplicationName { get; set; }
        public string TargetVersion { get; set; }
        public Constants.RollingUpgradeMode RollingUpgradeMode { get; set; }
        public bool ForceRestart { get; set; }
        public Constants.FailureAction FailureAction { get; set; }
        public int HealthCheckWaitDuration { get; set; }
        public int HealthCheckStableDuration { get; set; }
        public int HealthCheckRetryTimeout { get; set; }
    }
}