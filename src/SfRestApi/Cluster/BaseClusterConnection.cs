using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SfRestApi.Cluster
{
    public interface IClusterConnection
    {
        HttpClient HttpClient { get; }
        Uri CreateUri(string path);
    }

    public abstract class BaseClusterConnection : IClusterConnection
    {
        protected const string ApiVersion = "1.0";
        protected string Endpoint;
        protected int Port;
        protected string Scheme;

        public Uri CreateUri(string path)
        {
            return new UriBuilder
            {
                Scheme = Scheme,
                Host = Endpoint,
                Port = Port,
                Path = path
            }.Uri;
        }

        protected void CreateHttpClient(HttpClientHandler handler)
        {
            HttpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(10)
            };

            HttpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            HttpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }

        public HttpClient HttpClient { get; private set; }
    }
}