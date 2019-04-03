using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ncs.Prototype.Web.WebComposition.ViewModels;
using Polly.CircuitBreaker;

namespace Ncs.Prototype.Web.WebComposition.Controllers
{
    public class ApplicationController : Controller
    {
        private const string MainRenderViewName = "Application/RenderView";

        private readonly Dto.RegisteredApplicationsDto _registeredApplications;
        private readonly Services.IApplicationService _applicationService;
        private readonly ILogger<ApplicationController> _logger;

        private Models.PageViewModel _pageViewModel;

        public ApplicationController(IOptions<Dto.RegisteredApplicationsDto> registeredApplications, Services.IApplicationService applicationService, ILogger<ApplicationController> logger)
        {
            _registeredApplications = registeredApplications.Value;
            _applicationService = applicationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(IndexRequestViewModel requestViewModel)
        {
            try
            {
                await SetApplicationContextAsync(requestViewModel.RouteName);

                if (ModelState.IsValid)
                {
                    await _applicationService.GetEntrypointMarkUpAsync(_pageViewModel);
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

        [HttpGet]
        public async Task<IActionResult> Action(ActionGetRequestViewModel requestViewModel)
        {
            try
            {
                await SetApplicationContextAsync(requestViewModel.RouteName);

                if (ModelState.IsValid)
                {
                    string dataQuery = requestViewModel.Data?.Length > 0 ? string.Join("&data=", requestViewModel.Data) : "/" + requestViewModel.RouteName;

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
                    string data = Request.Query["data"];
                    var formParameters = (from a in requestViewModel.FormCollection select new KeyValuePair<string, string>(a.Key, a.Value)).ToArray();

                    await _applicationService.PostApplicationMarkUpAsync(_applicationService.Application.RootUrl + data, formParameters, _pageViewModel);
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
            _applicationService.Application = _registeredApplications.Applications?.FirstOrDefault(f => string.Compare(f.RouteName, routeName, true) == 0);

            if (_applicationService.Application == null)
            {
                string errorString = "Internal error: Missing Application definition";

                _logger.LogError(errorString);
                ModelState.AddModelError(string.Empty, errorString);
            }
            else
            {
                _logger.LogInformation($"Loaded application for name: {_applicationService.Application.Name}: {_applicationService.Application.MainMenuText}");

                _pageViewModel = MapApplicationToPageViewModel(_applicationService.Application);

                _applicationService.BearerToken = (User.Identity.IsAuthenticated ? await HttpContext.GetTokenAsync("id_token") : null);

                if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(_applicationService.BearerToken))
                {
                    var header = HttpContext.Request.Headers.FirstOrDefault(f => f.Key == "Authorization");

                    if (header.Key != null)
                    {
                        var parts = header.Value.First().Split(" ");

                        if (parts.Length > 1)
                        {
                            _applicationService.BearerToken = parts[1];
                        }
                    }
                }

                _applicationService.User = User;
                _applicationService.RequestBaseUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Content("~"));

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

            if (ModelState.IsValid && !string.IsNullOrEmpty(_applicationService.Application.HealthCheckUrl))
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