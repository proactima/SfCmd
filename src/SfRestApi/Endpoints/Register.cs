using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SfRestApi.Cluster;

namespace SfRestApi.Endpoints
{
    public class Register
    {
        private readonly ILogOutput _logger;
        private readonly IClusterConnection _clusterConnection;

        public Register(IClusterConnection clusterConnection, ILogOutput logger)
        {
            _clusterConnection = clusterConnection;
            _logger = logger;
        }

        public async Task<bool> RegisterAsync(string packageName)
        {
            try
            {
                var requestUriTemp = _clusterConnection.CreateUri($"/ApplicationTypes/$/Provision").ToString();
                var queryParams = new Dictionary<string, string>
                {
                    ["api-version"] = Constants.ApiVersion
                };
                var requestUri = QueryHelpers.AddQueryString(requestUriTemp, queryParams);

                var body = new Dictionary<string, object>
                {
                    ["ApplicationTypeBuildPath"] = packageName
                };

                var bodyContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                _logger.Log($"Registering {packageName}");
                var response =
                    await _clusterConnection.HttpClient.PostAsync(requestUri, bodyContent).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return false;
            }

            return true;
        }
    }
}