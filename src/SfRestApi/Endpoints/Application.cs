using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SfRestApi.Cluster;
using SfRestApi.Models.Endpoints;

namespace SfRestApi.Endpoints
{
    public class Application : BaseEndpoint
    {
        public Application(IClusterConnection clusterConnection, ILogOutput logger)
            : base(clusterConnection, logger)
        {
        }

        public async Task<bool> UpgradeAsync(ApplicationUpgrade upgradeOptions)
        {
            try
            {
                var requestUriTemp =
                    ClusterConnection.CreateUri($"/Applications/{upgradeOptions.ApplicationName}/$/Upgrade").ToString();
                var queryParams = new Dictionary<string, string>
                {
                    ["api-version"] = Constants.ApiVersion
                };
                var requestUri = QueryHelpers.AddQueryString(requestUriTemp, queryParams);

                var body = new Dictionary<string, object>
                {
                    ["Name"] = upgradeOptions.ApplicationName,
                    ["TargetApplicationTypeVersion"] = upgradeOptions.TargetVersion,
                    ["UpgradeKind"] = 1,
                    ["RollingUpgradeMode"] = (int) upgradeOptions.RollingUpgradeMode,
                    ["ForceRestart"] = upgradeOptions.ForceRestart,
                    ["MonitoringPolicy"] = new Dictionary<string, object>
                    {
                        ["FailureAction"] = (int) upgradeOptions.FailureAction,
                        ["HealthCheckWaitDurationInMilliseconds"] =
                        upgradeOptions.HealthCheckWaitDuration.ToMilliSeconds().ToString(),
                        ["HealthCheckStableDurationInMilliseconds"] =
                        upgradeOptions.HealthCheckStableDuration.ToMilliSeconds().ToString(),
                        ["HealthCheckRetryTimeoutInMilliseconds"] =
                        upgradeOptions.HealthCheckRetryTimeout.ToMilliSeconds().ToString(),
                    }
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                var response = await ClusterConnection.HttpClient
                    .PostAsync(requestUri, jsonContent)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }
    }
}