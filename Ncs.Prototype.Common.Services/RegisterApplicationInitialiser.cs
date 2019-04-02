using AspNetCore.AsyncInitialization;
using Ncs.Prototype.Dto;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ncs.Prototype.Common
{
    public class RegisterApplicationInitialiser : IAsyncInitializer
    {
        private readonly ApplicationDto _applicationDto;
        private readonly ApiManagementConfigurationDto _apiManagementConfiguration;

        public RegisterApplicationInitialiser(ApiManagementConfigurationDto apiManagementConfiguration, ApplicationDto applicationDto)
        {
            _apiManagementConfiguration = apiManagementConfiguration;
            _applicationDto = applicationDto;
        }

        public async Task InitializeAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsJsonAsync(_apiManagementConfiguration.RegisterUrl, _applicationDto);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
