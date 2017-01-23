using SfRestApi.Cluster;

namespace SfRestApi.Models
{
    public class NonSecureConnectionInfo : BaseClusterConnectionInfo
    {
        private NonSecureConnectionInfo(string endpoint, int port) : base(endpoint, port, false)
        {
        }

        public static NonSecureConnectionInfo Create(string hostname, int port)
        {
            return new NonSecureConnectionInfo(hostname, port);
        }

        public override IClusterConnection CreateClusterConnection()
        {
            var connection = new NonSecureClusterConnection();
            connection.Init(this);

            return connection;
        }
    }
}