using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ncs.Prototype.Web.Composition.Services;
using Ncs.Prototype.Web.Composition.ViewModels;
using Polly.CircuitBreaker;

namespace Ncs.Prototype.Web.Composition.Controllers
{
    public class ApplicationController : Controller
    {
        private const string MainRenderViewName = "Application/RenderView";

        private readonly ApplicationManagementService _applicationManagementService;
        private readonly IApplicationService _applicationService;
        private readonly ILogger<ApplicationController> _logger;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        private Models.PageViewModel _pageViewModel;

        public ApplicationController(ApplicationManagementService applicationManagementService, IApplicationService applicationService, ILogger<ApplicationController> logger, ICorrelationContextAccessor correlationContextAccessor)
        {
            _applicationManagementService = applicationManagementService;
            _applicationService = applicationService;
            _logger = logger;
            _correlationContextAccessor = correlationContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index(IndexRequestViewModel requestViewModel)
        {
            try
            {
                await SetApplicationContextAsync(requestViewModel.ApplicationName);

                if (ModelState.IsValid)
                {
                    await _applicationService.GetEntrypointMarkUpAsync(_pageViewModel);
                }
            }
            catch (BrokenCircuitException ex)
            {
                ModelState.AddModelError(string.Empty, $"{_applicationService.Application.MainMenuText}: {ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"{_applicationService.Application.MainMenuText}: {ex.Message}");
            }

            return View(MainRenderViewName, _pageViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Action(ActionGetRequestViewModel requestViewModel)
        {
            try
            {
                await SetApplicationContextAsync(requestViewModel.ApplicationName);

                if (ModelState.IsValid)
                {
                    string dataQuery = requestViewModel.Data.Length > 0 ? string.Join("&data=", requestViewModel.Data) : string.Empty;

                    await _applicationService.GetApplicationMarkUpAsync(dataQuery, _pageViewModel);
                }

            }
            catch (BrokenCircuitException ex)
            {
                ModelState.AddModelError(string.Empty, $"{_applicationService.Application.MainMenuText}: {ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"{_applicationService.Application.MainMenuText}: {ex.Message}");
            }

            return View(MainRenderViewName, _pageViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Action(ActionPostRequestViewModel requestViewModel)
        {
            try
            {
                await SetApplicationContextAsync(requestViewModel.ApplicationName);

                if (ModelState.IsValid)
                {
                    string data = Request.Query["data"];
                    var formParameters = (from a in requestViewModel.FormCollection select new KeyValuePair<string, string>(a.Key, a.Value)).ToArray();

                    await _applicationService.PostApplicationMarkUpAsync(_applicationService.Application.RootUrl + data, formParameters, _pageViewModel);
                }

            }
            catch (BrokenCircuitException ex)
            {
                ModelState.AddModelError(string.Empty, $"{_applicationService.Application.MainMenuText}: {ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"{_applicationService.Application.MainMenuText}: {ex.Message}");
            }

            return View(MainRenderViewName, _pageViewModel);
        }

        private async Task SetApplicationContextAsync(string applicationName)
        {
            var applications = await _applicationManagementService.GetApplications();

            _applicationService.Application = applications.FirstOrDefault(f => string.Compare(f.Name, applicationName, true) == 0);

            //ViewData["CorrelationId"] = _correlationContextAccessor.CorrelationContext.CorrelationId;

            //_logger.LogInformation($"Loaded application for name: {applicationName}: {_applicationService.Application.MainMenuText}: {_correlationContextAccessor.CorrelationContext.CorrelationId}");

            _pageViewModel = MapApplicationToPageViewModel(_applicationService.Application);

            _applicationService.BearerToken = (User.Identity.IsAuthenticated ? await HttpContext.GetTokenAsync("id_token") : null);
            _applicationService.User = User;
            _applicationService.RequestBaseUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Content("~"));

            if (_applicationService.Application == null)
            {
                ModelState.AddModelError(string.Empty, "Internal error: Missing Application definition");
            }
            else
            {
                if (string.IsNullOrEmpty(_applicationService.Application.RootUrl))
                {
                    ModelState.AddModelError(string.Empty, $"Application context error: ({_applicationService.Application.Title}) Missing RootUrl definition");
                }

                if (string.IsNullOrEmpty(_applicationService.Application.HealthCheckUrl))
                {
                    ModelState.AddModelError(string.Empty, $"Application context error: ({_applicationService.Application.Title}) Missing HealthCheckUrl definition");
                }

                if (string.IsNullOrEmpty(_applicationService.Application.EntrypointUrl))
                {
                    ModelState.AddModelError(string.Empty, $"Application context error: ({_applicationService.Application.Title}) Missing EntrypointUrl definition");
                }

                if (_applicationService.Application.ShowSideBar && string.IsNullOrEmpty(_applicationService.Application.SidebarUrl))
                {
                    ModelState.AddModelError(string.Empty, $"Application context error: ({_applicationService.Application.Title}) Missing SidebarUrl definition");
                }
            }
            /*
            if (ModelState.IsValid && !string.IsNullOrEmpty(_applicationService.Application.HealthCheckUrl))
            {
                await _applicationService.HeathCheckAsync();

                if (!_applicationService.Health.IsHealthy)
                {
                    ModelState.AddModelError(string.Empty, $"{_applicationService.Health.UnHealthyClue}: ({_applicationService.Application.Title})");
                }
            }*/
        }

        private Models.PageViewModel MapApplicationToPageViewModel(Dto.ApplicationDto application)
        {
            // this could be managed by AutoMapper if we add it's NuGet - but so far this is the only mapping requirement.
            // if AutoMapper is added, then this method should be replaced with a mapper operation
            var pageViewModel = new Models.PageViewModel()
            {
                LayoutName = application?.LayoutName,
                PageTitle = application?.Title,
                Branding = application?.Branding
            };

            return pageViewModel;
        }

    }
}