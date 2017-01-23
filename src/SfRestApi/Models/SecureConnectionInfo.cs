using System.Security.Cryptography.X509Certificates;
using SfRestApi.Cluster;

namespace SfRestApi.Models
{
    public class SecureConnectionInfo : BaseClusterConnectionInfo
    {
        public SecureConnectionInfo(string endpoint, int port) : base(endpoint, port, true)
        {
        }

        public string CertificateThumbprint { get; private set; }
        public StoreName StoreName { get; private set; }
        public StoreLocation StoreLocation { get; private set; }

        public static BaseClusterConnectionInfo CreateSecure(
            string hostname,
            int port,
            string certThumbprint,
            StoreName storeName = StoreName.My,
            StoreLocation storeLocation = StoreLocation.CurrentUser)
        {
            return new SecureConnectionInfo(hostname, port)
            {
                CertificateThumbprint = certThumbprint,
                StoreName = storeName,
                StoreLocation = storeLocation
            };
        }

        public override IClusterConnection CreateClusterConnection()
        {
            var connection = new SecureClusterConnection();
            connection.Init(this);

            return connection;
        }
    }
}