using System.Net.Http;
using SfRestApi.Models;

namespace SfRestApi.Cluster
{
    public class NonSecureClusterConnection : BaseClusterConnection
    {
        public void Init(BaseClusterConnectionInfo connectionInfo)
        {
            Endpoint = connectionInfo.Endpoint;
            Port = connectionInfo.Port;
            Scheme = "http";

            var handler = new HttpClientHandler();
            CreateHttpClient(handler);
        }
    }
}