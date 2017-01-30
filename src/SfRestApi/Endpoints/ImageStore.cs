using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Polly;
using Polly.Retry;
using SfRestApi.Cluster;

namespace SfRestApi.Endpoints
{
    public class ImageStore : BaseEndpoint
    {
        private readonly RetryPolicy<HttpResponseMessage> retryPolicy;

        public ImageStore(IClusterConnection clusterConnection, ILogOutput logger)
            : base(clusterConnection, logger)
        {
            retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(5, i => TimeSpan.FromSeconds(Math.Pow(2, i)));
        }

        public async Task DeleteAsync(string packageName)
        {
            try
            {
                var requestUriTemp = ClusterConnection.CreateUri($"/ImageStore/{packageName}").ToString();
                var queryParams = new Dictionary<string, string>
                {
                    ["api-version"] = Constants.ApiVersion
                };
                var requestUri = QueryHelpers.AddQueryString(requestUriTemp, queryParams);

                var response = await ClusterConnection.HttpClient.DeleteAsync(requestUri).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task ListAsync(string packageName = null)
        {
            try
            {
                var requestUriTemp = string.IsNullOrWhiteSpace(packageName)
                    ? ClusterConnection.CreateUri($"/ImageStore")
                    : ClusterConnection.CreateUri($"/ImageStore/{packageName}");

                var queryParams = new Dictionary<string, string>
                {
                    ["api-version"] = Constants.ApiVersion
                };
                var requestUri = QueryHelpers.AddQueryString(requestUriTemp.ToString(), queryParams);

                var response = await ClusterConnection.HttpClient.GetAsync(requestUri).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<bool> UploadAsync(FileInfo file, string fileInImageStore, string appPackagePathInStore)
        {
            try
            {
                var fileRelativePath = $"{appPackagePathInStore}\\{fileInImageStore}";
                var requestUriTemp =
                    ClusterConnection.CreateUri($"/ImageStore/{appPackagePathInStore}\\{fileInImageStore}").ToString();

                var queryParams = new Dictionary<string, string>
                {
                    ["api-version"] = Constants.ApiVersion
                };
                var requestUri = QueryHelpers.AddQueryString(requestUriTemp, queryParams);

                ByteArrayContent byteContent;

                // Handle the _.dir marker files
                if (file.Name.Equals("_.dir", StringComparison.CurrentCultureIgnoreCase))
                {
                    byteContent = new ByteArrayContent(new byte[0]);
                }
                else
                {
                    using (
                        var sourceStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.None)
                    )
                    {
                        var data = new byte[file.Length];
                        await sourceStream.ReadAsync(data, 0, data.Length).ConfigureAwait(false);
                        byteContent = new ByteArrayContent(data);
                    }
                }

                Logger.Log($"Uploading {file.FullName} as {fileRelativePath}");

                var response = await retryPolicy
                    .ExecuteAsync(() => ClusterConnection.HttpClient.PutAsync(requestUri, byteContent))
                    .ConfigureAwait(false);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
        }
    }
}