using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Ncs.Prototype.Dto;

namespace Ncs.Prototype.Web.WebComposition.Services
{
    public interface IApplicationService
    {
        ApplicationDto Application { get; set; }
        string BearerToken { get; set; }
        ClaimsPrincipal User { get; set; }
        string RequestBaseUrl { get; set; }
        (bool IsHealthy, string UnHealthyClue) Health { get; }

        Task GetEntrypointMarkUpAsync(Models.PageViewModel pageViewModel);
        Task GetApplicationMarkUpAsync(string data, Models.PageViewModel pageViewModel);
        Task PostApplicationMarkUpAsync(string url, KeyValuePair<string, string>[] formParameters, Models.PageViewModel pageViewModel);
        Task HeathCheckAsync();
    }
}