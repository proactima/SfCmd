namespace SfRestApi
{
    public static class Constants
    {
        public const string ApiVersion = "1.0";

        public enum RollingUpgradeMode
        {
            Invalid = 0,
            UnmonitoredAuto = 1,
            UnmonitoredManual = 2,
            Monitored = 3
        };

        public enum FailureAction
        {
            Invalid = 0,
            Rollback = 1,
            Manual  = 2
        }
    }
}