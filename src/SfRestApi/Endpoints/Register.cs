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
    public class Register : BaseEndpoint
    {
        public Register(IClusterConnection clusterConnection, ILogOutput logger)
            : base(clusterConnection, logger)
        {
        }

        public async Task<bool> RegisterAsync(string packageName)
        {
            try
            {
                var requestUriTemp = ClusterConnection.CreateUri($"/ApplicationTypes/$/Provision").ToString();
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

                Logger.Log($"Registering {packageName}");
                var response =
                    await ClusterConnection.HttpClient.PostAsync(requestUri, bodyContent).ConfigureAwait(false);

                if(!response.IsSuccessStatusCode)
                {
                    try
                    {
                        var errorBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        Console.WriteLine("Things exploded...");
                        Console.WriteLine(errorBody);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }

            return true;
        }
    }
}