using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using test_poc.Models;

namespace feature_flags.Controllers
{
    public class HomeController : Controller
    {
         private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [Route("appConfigurationPoc")]
        [HttpGet]
        public JsonResult Configuration()
        {
            var lstConfigs = _configuration.AsEnumerable().ToList();

            List<FeatureFlag> lstFeatureFlags = new List<FeatureFlag>();

            foreach (var item in lstConfigs)
            {
                if (item.Key.Contains(".appconfig.featureflag"))
                    lstFeatureFlags.Add(JsonConvert.DeserializeObject<FeatureFlag>(item.Value));
            }

            return Json(lstFeatureFlags);
        }
    }
}
