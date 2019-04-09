using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ncs.Prototype.Common;

namespace Ncs.Prototype.Web.Composition.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;
        protected readonly ICorrelationContextAccessor _correlationContextAccessor;

        public BaseController(ILogger<BaseController> logger, ICorrelationContextAccessor correlationContextAccessor)
        {
            _logger = logger;
            _correlationContextAccessor = correlationContextAccessor;
        }

        protected string BaseUrl()
        {
            return string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Content("~"));
        }

        protected async Task<string> GetBearerTokenAsync()
        {
            return User.Identity.IsAuthenticated ? await HttpContext.GetTokenAsync(Constants.BearerTokenName) : null;
        }
    }
}