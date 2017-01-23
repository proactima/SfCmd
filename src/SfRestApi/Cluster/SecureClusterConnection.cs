using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using SfRestApi.Models;

namespace SfRestApi.Cluster
{
    public class SecureClusterConnection : BaseClusterConnection
    {
        private X509Certificate2 _cert;

        public void Init(BaseClusterConnectionInfo connectionInfo)
        {
            Endpoint = connectionInfo.Endpoint;
            Port = connectionInfo.Port;
            Scheme = "https";

            var connInfo = connectionInfo as SecureConnectionInfo;
            if (connInfo == null)
                throw new Exception("Something went wrong");

            using (var certStore = new X509Store(connInfo.StoreName, connInfo.StoreLocation))
            {
                certStore.Open(OpenFlags.ReadOnly);
                var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint,
                    connInfo.CertificateThumbprint, false);

                switch (certCollection.Count)
                {
                    case 0:
                        throw new Exception("Did not find certificate");
                    case 1:
                        _cert = certCollection[0];
                        break;
                    default:
                        throw new Exception("Found more than one certificate. Aborting.");
                }
            }

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(_cert);
            handler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;

            CreateHttpClient(handler);
        }
    }
}