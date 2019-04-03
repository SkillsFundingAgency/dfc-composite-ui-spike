using CorrelationId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    }
}