using SfRestApi.Cluster;

namespace SfRestApi.Endpoints
{
    public abstract class BaseEndpoint
    {
        protected readonly IClusterConnection ClusterConnection;
        protected readonly ILogOutput Logger;

        protected BaseEndpoint(IClusterConnection clusterConnection, ILogOutput logger)
        {
            ClusterConnection = clusterConnection;
            Logger = logger;
        }
    }
}