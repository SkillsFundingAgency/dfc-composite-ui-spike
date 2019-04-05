using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ncs.Prototype.Web.Composition.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.Composition.ViewComponents
{
    public class ApplicationNavigationMenuViewComponent : ViewComponent
    {
        private readonly ApplicationManagementService _applicationManagementService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationNavigationMenuViewComponent(ApplicationManagementService applicationManagementService, IHttpContextAccessor httpContextAccessor)
        {
            _applicationManagementService = applicationManagementService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new ApplicationNavigationMenuViewModel();
            vm.SessionId = _httpContextAccessor.HttpContext.Session.Id;
            var applications = await _applicationManagementService.GetApplications();
            vm.Applications = applications.Where(x => x.IsOnline && x.IsRegistered && x.IsHealthy).ToList();
            return View(vm);
        }

    }
}
