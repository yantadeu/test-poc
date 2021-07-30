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

namespace feature_flags
{
    public static class feature_flags
    {
       [FunctionName("feature_flags")]
        public static async Task<JsonResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
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
    }
}
