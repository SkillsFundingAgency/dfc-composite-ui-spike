using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ncs.Prototype.Web.Composition.Services;
using Ncs.Prototype.Web.Composition.ViewModels;
using Polly.CircuitBreaker;

namespace Ncs.Prototype.Web.Composition.Controllers
{
    public class ApplicationController : BaseController
    {
        private const string MainRenderViewName = "Application/RenderView";

        private readonly ApplicationManagementService _applicationManagementService;
        private readonly IApplicationService _applicationService;

        private Models.PageViewModel _pageViewModel;

        public ApplicationController(ApplicationManagementService applicationManagementService, IApplicationService applicationService, ILogger<ApplicationController> logger, ICorrelationContextAccessor correlationContextAccessor) : base(logger, correlationContextAccessor)
        {
            _applicationManagementService = applicationManagementService;
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<IActionResult> Action(ActionGetRequestViewModel requestViewModel)
        {
            try
            {
                await SetApplicationContextAsync(requestViewModel.RouteName);

                if (ModelState.IsValid)
                {
                    string dataQuery = $"/{requestViewModel.RouteName}/{requestViewModel.Data}";

                    if (!string.IsNullOrEmpty(_applicationService.Application.ChildRoutePrefix)) 
                    {
                        dataQuery = $"/{_applicationService.Application.ChildRoutePrefix}/{requestViewModel.Data}";
                    }

                    await _applicationService.GetApplicationMarkUpAsync(dataQuery, _pageViewModel);
                }

            }
            catch (BrokenCircuitException ex)
            {
                string errorString = $"{_applicationService.Application.MainMenuText}: BrokenCircuit: {ex.Message}";

                _logger.LogError(ex, errorString);
                ModelState.AddModelError(string.Empty, errorString);
            }
            catch (Exception ex)
            {
                string errorString = $"{_applicationService.Application.MainMenuText}: {ex.Message}";

                _logger.LogError(ex, errorString);
                ModelState.AddModelError(string.Empty, errorString);
            }

            return View(MainRenderViewName, _pageViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Action(ActionPostRequestViewModel requestViewModel)
        {
            try
            {
                await SetApplicationContextAsync(requestViewModel.RouteName);

                if (ModelState.IsValid)
                {
                    var formParameters = (from a in requestViewModel.FormCollection select new KeyValuePair<string, string>(a.Key, a.Value)).ToArray();

                    await _applicationService.PostApplicationMarkUpAsync(_applicationService.Application.RootUrl + Request.Path, formParameters, _pageViewModel);
                }

            }
            catch (BrokenCircuitException ex)
            {
                string errorString = $"{_applicationService.Application.MainMenuText}: BrokenCircuit: {ex.Message}";

                _logger.LogError(ex, errorString);
                ModelState.AddModelError(string.Empty, errorString);
            }
            catch (Exception ex)
            {
                string errorString = $"{_applicationService.Application.MainMenuText}: {ex.Message}";

                _logger.LogError(ex, errorString);
                ModelState.AddModelError(string.Empty, errorString);
            }

            return View(MainRenderViewName, _pageViewModel);
        }

        private async Task SetApplicationContextAsync(string routeName)
        {
            var applications = await _applicationManagementService.GetApplications();

            _applicationService.Application = applications.FirstOrDefault(f => string.Compare(f.RouteName, routeName, true) == 0);

            if (_applicationService.Application == null)
            {
                string errorString = "Internal error: Missing Application definition";

                _logger.LogError(errorString);
                ModelState.AddModelError(string.Empty, errorString);
            }
            else
            {
                ViewData["CorrelationId"] = _correlationContextAccessor.CorrelationContext.CorrelationId;

                _logger.LogInformation($"Loaded application for name: {_applicationService.Application.Name}: {_applicationService.Application.MainMenuText}: {_correlationContextAccessor.CorrelationContext.CorrelationId}");

                _pageViewModel = MapApplicationToPageViewModel(_applicationService.Application);

                _applicationService.BearerToken = await GetBearerTokenAsync();
                _applicationService.User = User;
                _applicationService.RequestBaseUrl = BaseUrl();

                if (string.IsNullOrEmpty(_applicationService.Application.RootUrl))
                {
                    string errorString = $"Application context error: ({_applicationService.Application.Title}) Missing RootUrl definition";

                    _logger.LogError(errorString);
                    ModelState.AddModelError(string.Empty, errorString);
                }

                if (string.IsNullOrEmpty(_applicationService.Application.HealthCheckUrl))
                {
                    string errorString = $"Application context error: ({_applicationService.Application.Title}) Missing HealthCheckUrl definition";

                    _logger.LogError(errorString);
                    ModelState.AddModelError(string.Empty, errorString);
                }

                if (string.IsNullOrEmpty(_applicationService.Application.EntrypointUrl))
                {
                    string errorString = $"Application context error: ({_applicationService.Application.Title}) Missing EntrypointUrl definition";

                    _logger.LogError(errorString);
                    ModelState.AddModelError(string.Empty, errorString);
                }

                if (_applicationService.Application.ShowSideBar && string.IsNullOrEmpty(_applicationService.Application.SidebarUrl))
                {
                    string errorString = $"Application context error: ({_applicationService.Application.Title}) Missing SidebarUrl definition";

                    _logger.LogError(errorString);
                    ModelState.AddModelError(string.Empty, errorString);
                }
            }

            if (ModelState.IsValid && !string.IsNullOrEmpty(_applicationService.Application?.HealthCheckUrl))
            {
                await _applicationService.HeathCheckAsync();

                if (!_applicationService.Health.IsHealthy)
                {
                    string errorString = $"{_applicationService.Health.UnHealthyClue}: ({_applicationService.Application.Title})";

                    _logger.LogError(errorString);
                    ModelState.AddModelError(string.Empty, errorString);
                }
            }
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