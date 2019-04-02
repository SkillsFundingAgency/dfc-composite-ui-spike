using Microsoft.AspNetCore.Mvc;
using Ncs.Prototype.Web.Composition.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.Composition.ViewComponents
{
    public class ListApplicationsViewComponent : ViewComponent
    {
        private readonly ApplicationManagementService _applicationManagementService;

        public ListApplicationsViewComponent(ApplicationManagementService applicationManagementService)
        {
            _applicationManagementService = applicationManagementService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new ListApplicationsViewModel();
            var applications = await _applicationManagementService.GetApplications();
            vm.Applications = applications.Where(x => x.IsOnline && x.IsRegistered && x.IsHealthy).ToList();
            return View(vm);
        }

    }
}
