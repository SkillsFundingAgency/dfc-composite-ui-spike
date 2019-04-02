using Microsoft.AspNetCore.Mvc;
using Ncs.Prototype.Common;
using Ncs.Prototype.Dto;
using Ncs.Prototype.Web.ApplicationManagement.Dto;
using Ncs.Prototype.Web.ApplicationManagement.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.ApplicationManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationService _applicationService;

        public ApplicationController(ApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterApplicationRequestDto requestDto)
        {
            var appModel = Convert(requestDto);
            await _applicationService.Register(appModel);
            return Ok();
        }

        [Route("Unregister")]
        [HttpPost]
        public async Task<IActionResult> Unregister(UnregisterApplicationRequestDto requestDto)
        {
            await _applicationService.Unregister(requestDto.Name);
            return Ok();
        }

        [Route("enable")]
        [HttpPost]
        public async Task<IActionResult> Enable(EnableApplicationRequestDto requestDto)
        {
            await _applicationService.Enable(requestDto.Name);
            return Ok();
        }

        [Route("disable")]
        [HttpPost]
        public async Task<IActionResult> Disable(DisableApplicationRequestDto requestDto)
        {
            await _applicationService.Disable(requestDto.Name);
            return Ok();
        }

        [Route("getAllApplications")]
        [HttpGet]
        public async Task<IActionResult> GetAllApplications()
        {
            var applications = await _applicationService.GetAllApplications();
            var dtos = Convert(applications);
            return Ok(dtos);
        }

        [Route("getOnlineApplications")]
        [HttpGet]
        public async Task<IActionResult> GetOnlineApplications()
        {
            var applications = await _applicationService.GetOnlineApplications();
            var dtos = Convert(applications);
            return Ok(dtos);
        }

        [Route("getRegisteredApplication")]
        [HttpGet]
        public async Task<IActionResult> GetRegisteredApplications()
        {
            var applications = await _applicationService.GetRegisteredApplication();
            var dtos = Convert(applications);
            return Ok(dtos);
        }

        [Route("getOfflineApplications")]
        [HttpGet]
        public async Task<IActionResult> GetOfflineApplications()
        {
            var applications = await _applicationService.GetRegisteredApplication();
            var dtos = Convert(applications);
            return Ok(dtos);
        }

        private ApplicationEntity Convert(RegisterApplicationRequestDto source)
        {
            var result = new ApplicationEntity();
            result.AppNavBarUrl = source.AppNavBarUrl;
            result.BackButtonUrl = source.BackButtonUrl;
            result.Branding = source.Branding;
            result.BreadcrumbsUrl = source.BreadcrumbsUrl;
            result.Description = source.Description;
            result.EntrypointUrl = source.EntrypointUrl;
            result.HealthCheckUrl = source.HealthCheckUrl;
            result.LayoutName = source.LayoutName;
            result.MainMenuText = source.MainMenuText;
            result.Name = source.Name;
            result.PersonalisationUrl = source.PersonalisationUrl;
            result.RequiresAuthorization = source.RequiresAuthorization;
            result.RootUrl = source.RootUrl;
            result.RouteName = source.RouteName;
            result.ShowSideBar = source.ShowSideBar;
            result.SidebarUrl = source.SidebarUrl;
            result.Title = source.Title;
            return result;
        }

        private List<ApplicationDto> Convert(List<ApplicationEntity> source)
        {
            var result = new List<ApplicationDto>();
            source.ForEach(x => result.Add(Convert(x)));
            return result;
        }

        private ApplicationDto Convert(ApplicationEntity source)
        {
            var result = new ApplicationDto();
            result.AppNavBarUrl = source.AppNavBarUrl;
            result.IsOnline = source.IsOnline;
            result.IsRegistered = source.IsRegistered;
            result.IsHealthy = source.IsHealthy;
            result.BackButtonUrl = source.BackButtonUrl;
            result.Branding = source.Branding;
            result.BreadcrumbsUrl = source.BreadcrumbsUrl;
            result.Description = source.Description;
            result.EntrypointUrl = source.EntrypointUrl;
            result.HealthCheckUrl = source.HealthCheckUrl;
            result.LayoutName = source.LayoutName;
            result.MainMenuText = source.MainMenuText;
            result.Name = source.Name;
            result.PersonalisationUrl = source.PersonalisationUrl;
            result.RequiresAuthorization = source.RequiresAuthorization;
            result.RootUrl = source.RootUrl;
            result.RouteName = source.RouteName;
            result.ShowSideBar = source.ShowSideBar;
            result.SidebarUrl = source.SidebarUrl;
            result.Title = source.Title;
            return result;
        }

    }
}
