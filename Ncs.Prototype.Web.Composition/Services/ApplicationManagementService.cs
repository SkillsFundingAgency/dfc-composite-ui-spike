using Microsoft.Extensions.Caching.Memory;
using Ncs.Prototype.Common;
using Ncs.Prototype.Dto;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.Composition.Services
{
    public class ApplicationManagementService
    {
        private readonly ApiManagementConfigurationDto _apiManagementConfiguration;
        private readonly IMemoryCache _memoryCache;

        public ApplicationManagementService(ApiManagementConfigurationDto apiManagementConfiguration, IMemoryCache memoryCache)
        {
            _apiManagementConfiguration = apiManagementConfiguration;
            _memoryCache = memoryCache;
        }
        
        public async Task<List<ApplicationDto>> GetApplications()
        {
            var applications = new List<ApplicationDto>();
            _memoryCache.TryGetValue(CacheKey.Applications, out applications);

            if (applications == null)
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(_apiManagementConfiguration.BaseUrl + "api/application/GetAllApplications");
                    response.EnsureSuccessStatusCode();
                    applications = await response.As<List<ApplicationDto>>();
                    _memoryCache.Set(CacheKey.Applications, applications);
                }
            }

            return applications;
        }

        public async Task Enable(string applicationName)
        {
            using (var httpClient = new HttpClient())
            {
                var dto = new EnableApplicationRequestDto() { Name = applicationName };
                var response = await httpClient.PostAsJsonAsync(_apiManagementConfiguration.BaseUrl + "api/application/enable", dto);
                response.EnsureSuccessStatusCode();
                _memoryCache.Remove(CacheKey.Applications);
            }
        }

        public async Task Disable(string applicationName)
        {
            using (var httpClient = new HttpClient())
            {
                var dto = new DisableApplicationRequestDto() { Name = applicationName };
                var response = await httpClient.PostAsJsonAsync(_apiManagementConfiguration.BaseUrl + "api/application/disable", dto);
                response.EnsureSuccessStatusCode();
                _memoryCache.Remove(CacheKey.Applications);
            }
        }
    }

}