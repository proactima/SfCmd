using SfRestApi.Cluster;

namespace SfRestApi.Models
{
    public abstract class BaseClusterConnectionInfo
    {
        protected BaseClusterConnectionInfo(string endpoint, int port, bool isSecure)
        {
            Endpoint = endpoint;
            Port = port;
            IsSecure = isSecure;
        }

        public string Endpoint { get; private set; }
        public int Port { get; private set; }
        public bool IsSecure { get; private set; }

        public abstract IClusterConnection CreateClusterConnection();
    }
}