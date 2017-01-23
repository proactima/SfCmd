using ServiceFabricUploader.Models;
using SfRestApi.Models;

namespace ServiceFabricUploader
{
    public static class ConnectionInfoHelper
    {
        internal static BaseClusterConnectionInfo CreateConnectionInfo(AppOptions options)
        {
            if (options.SecureCluster)
            {
                return SecureConnectionInfo.CreateSecure(
                    options.ClusterHostname,
                    options.ClusterPort,
                    options.CertificateThumbprint,
                    options.CertificateStore,
                    options.CertificateLocation);
            }

            return NonSecureConnectionInfo.Create(options.ClusterHostname, options.ClusterPort);
        }
    }
}
