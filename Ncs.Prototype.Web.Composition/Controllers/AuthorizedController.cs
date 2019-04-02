using CorrelationId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Ncs.Prototype.Web.Composition.Services;

namespace Ncs.Prototype.Web.Composition.Controllers
{
    [Authorize]
    public class AuthorizedController : ApplicationController
    {
        public AuthorizedController(
            ApplicationManagementService applicationManagementService,
            Services.IApplicationService applicationService,
            ILogger<AuthorizedController> logger,
            ICorrelationContextAccessor correlationContextAccessor
            ) : base(applicationManagementService, applicationService, logger, correlationContextAccessor) { }
    }
}