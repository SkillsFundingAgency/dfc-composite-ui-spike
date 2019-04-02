using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ncs.Prototype.Web.WebComposition.Controllers
{
    [Authorize]
    public class AuthorizedController : ApplicationController
    {
        public AuthorizedController(IOptions<Dto.RegisteredApplicationsDto> applications, Services.IApplicationService applicationService, ILogger<AuthorizedController> logger) : base(applications, applicationService, logger) { }
    }
}