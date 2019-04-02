using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ncs.Prototype.Common;
using Ncs.Prototype.Dto;
using Ncs.Prototype.Web.Composition.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.Composition.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly IMemoryCache _memoryCache;
        private readonly ApplicationManagementService _applicationManagementService;

        public HealthController(HealthCheckService healthCheckService, IMemoryCache memoryCache, ApplicationManagementService applicationManagementService)
        {
            _healthCheckService = healthCheckService;
            _memoryCache = memoryCache;
            _applicationManagementService = applicationManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var healthReport = await _healthCheckService.CheckHealthAsync();

            var applications = await _applicationManagementService.GetApplications();
            if (applications != null)
            {
                foreach (var entry in healthReport.Entries)
                {
                    var app = applications.FirstOrDefault(x => x.Name == entry.Key);
                    if (app != null)
                    {
                        if (entry.Value.Status == HealthStatus.Unhealthy)
                        {
                            app.IsHealthy = false;
                        }
                        else if (entry.Value.Status == HealthStatus.Healthy)
                        {
                            app.IsHealthy = true;
                        }
                    }
                }
                _memoryCache.Remove(CacheKey.Applications);
                _memoryCache.Set(CacheKey.Applications, applications);
            }

            return Ok(healthReport);
        }

    }
}