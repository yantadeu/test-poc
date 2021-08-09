using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using test_poc.Models;
using Azure.Data.AppConfiguration;
using System;

namespace feature_flags
{
    public static class feature_flags
    {

       [FunctionName("consultar_feature_flags")]
        public static async Task<JsonResult> GetFeatureFlags(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var config = new ConfigurationBuilder()
                .AddAzureAppConfiguration("Endpoint=https://appcs-plataformadigital-qas.azconfig.io;Id=pzi5-le-s0:yuMvoKnxtcRqUqWYWYss;Secret=nEZtPEYtgPS57jc8dBl6w6zORYZTdX+vuETdky54deg=")
                .Build();

            var lstConfigs = config.AsEnumerable().ToList();

            List<FeatureFlag> lstFeatureFlags = new List<FeatureFlag>();

            foreach (var item in lstConfigs)
            {
                if (item.Key.Contains(".appconfig.featureflag"))
                    lstFeatureFlags.Add(JsonConvert.DeserializeObject<FeatureFlag>(item.Value));
            }

            return new JsonResult(lstFeatureFlags);
        }

        [FunctionName("enabled_feature_flags")]
        public static async Task<JsonResult> EnabledFeatureFlag(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string feature = req.Query["feature"];
            bool enabled = Convert.ToBoolean(req.Query["enabled"]);

            string connectionString = "Endpoint=https://appcs-plataformadigital-qas.azconfig.io;Id=pzi5-le-s0:yuMvoKnxtcRqUqWYWYss;Secret=nEZtPEYtgPS57jc8dBl6w6zORYZTdX+vuETdky54deg=";

            var configuration = new ConfigurationBuilder().AddAzureAppConfiguration(connectionString).Build();

            var client = new ConfigurationClient(connectionString);
            
            var lstConfigs = configuration.AsEnumerable().ToList();
            
            var featureFlag = new FeatureFlag();
             
            foreach (var item in lstConfigs)
            {
                if (item.Key.Contains(feature))
                {
                    featureFlag = JsonConvert.DeserializeObject<FeatureFlag>(item.Value);

                    var config = new FeatureFlagConfigurationSetting(feature, enabled);
                    featureFlag.enabled = enabled;
                    config.Value = JsonConvert.SerializeObject(featureFlag);
                    ConfigurationSetting setting = client.SetConfigurationSetting(config);
                }
            }

            return new JsonResult(featureFlag);
        } 
    }
}
