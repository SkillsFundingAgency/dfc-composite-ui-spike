using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ncs.Prototype.Dto;
using Ncs.Prototype.Dto.Sitemap;
using Ncs.Prototype.Web.Composition.Services;
using Polly.CircuitBreaker;

namespace Ncs.Prototype.Web.Composition.Controllers
{
    public class SitemapController : BaseController
    {
        private readonly ApplicationManagementService _applicationManagementService;

        public SitemapController(ApplicationManagementService applicationManagementService, ILogger<SitemapController> logger, ICorrelationContextAccessor correlationContextAccessor) : base(logger, correlationContextAccessor)
        {
            _applicationManagementService = applicationManagementService;
        }

        [HttpGet]
        public async Task<ContentResult> Sitemap()
        {
            try
            {
                _logger.LogInformation("Generating Sitemap");

                var sitemap = GenerateThisSiteSitemap();

                // get all the registered application site maps
                await GetApplicationSitemapsAsync(sitemap);

                string xmlString = sitemap.WriteSitemapToString();

                _logger.LogInformation("Generated Sitemap");

                return Content(xmlString, "application/xml");
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, $"{nameof(Sitemap)}: BrokenCircuit: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Sitemap)}: {ex.Message}");
            }

            return null;
        }

        private Sitemap GenerateThisSiteSitemap()
        {
            const string homeControllerName = "Home";
            var sitemap = new Sitemap();

            // output the composite UI site maps
            sitemap.Add(new SitemapLocation() { Url = Url.Action(nameof(HomeController.Index), homeControllerName, null, Request.Scheme), Priority = 1 });
            sitemap.Add(new SitemapLocation() { Url = Url.Action(nameof(HomeController.Privacy), homeControllerName, null, Request.Scheme), Priority = 1 });

            return sitemap;
        }

        private async Task GetApplicationSitemapsAsync(Sitemap sitemap)
        {
            // loop through the registered applications and create some tasks - one per application that has a sitemap url
            var applications = await _applicationManagementService.GetApplications();
            var applicationSitemapServices = await CreateApplicationSitemapServiceTasksAsync(applications);

            // await all application sitemap service tasks to complete
            var allTasks = (from a in applicationSitemapServices select a.TheTask).ToArray();

            await Task.WhenAll(allTasks);

            OutputApplicationsSitemaps(sitemap, applications, applicationSitemapServices);
        }

        private async Task<List<IApplicationSitemapService>> CreateApplicationSitemapServiceTasksAsync(List<ApplicationDto> applications)
        {
            // loop through the registered applications and create some tasks - one per application that has a sitemap url
            var applicationSitemapServices = new List<IApplicationSitemapService>();
            string bearerToken = await GetBearerTokenAsync();

            foreach (var application in applications.Where(w => !string.IsNullOrEmpty(w.SitemapUrl)))
            {
                var applicationSitemapService = HttpContext.RequestServices.GetService(typeof(IApplicationSitemapService)) as ApplicationSitemapService;

                applicationSitemapService.BearerToken = bearerToken;
                applicationSitemapService.SitemapUrl = application.SitemapUrl;
                applicationSitemapService.TheTask = applicationSitemapService.GetAsync();

                applicationSitemapServices.Add(applicationSitemapService);
            }

            return applicationSitemapServices;
        }

        private void OutputApplicationsSitemaps(Sitemap sitemap, List<ApplicationDto> applications, List<IApplicationSitemapService> applicationSitemapServices)
        {
            string baseUrl = BaseUrl();

            // get the task results as individual sitemaps and merge into one
            foreach (var applicationSiteMap in applicationSitemapServices)
            {
                if (applicationSiteMap.TheTask.IsCompletedSuccessfully)
                {
                    var mappings = applicationSiteMap.TheTask.Result;

                    if (mappings.Count() > 0)
                    {
                        foreach (var mapping in mappings)
                        {
                            // rewrite the URL to swap any child application address prefix for the composite UI address prefix
                            foreach (var application in applications)
                            {
                                if (mapping.Url.StartsWith(application.RootUrl,StringComparison.InvariantCultureIgnoreCase))
                                {
                                    mapping.Url = mapping.Url.Replace(application.RootUrl, baseUrl, StringComparison.InvariantCultureIgnoreCase);
                                }
                            }
                        }

                        sitemap.AddRange(mappings);
                    }
                }
            }
        }
    }
}