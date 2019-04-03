using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

                string baseUrl = BaseUrl();
                var sitemap = new Dto.Sitemap.Sitemap();

                // output the composite UI site maps
                sitemap.Add(new SitemapLocation() { Url = $"{baseUrl}", Priority = 1 });
                sitemap.Add(new SitemapLocation() { Url = $"{baseUrl}/home", Priority = 1 });
                sitemap.Add(new SitemapLocation() { Url = $"{baseUrl}/home/privacy", Priority = 1 });

                // get all the registered application site maps
                var applicationsSitemap = await GetApplicationSitemapsAsync();

                if (applicationsSitemap?.Locations.Count() > 0)
                {
                    sitemap.AddRange(applicationsSitemap.Locations);
                }

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

        private async Task<Dto.Sitemap.Sitemap> GetApplicationSitemapsAsync()
        {
            // loop through the registered applications and create some tasks - one per application that has a sitemap url
            string baseUrl = BaseUrl();
            var applicationSitemapServices = new List<IApplicationSitemapService>();
            var applications = await _applicationManagementService.GetApplications();

            foreach (var application in applications.Where(w => !string.IsNullOrEmpty(w.SitemapUrl)))
            {
                var applicationSitemapService = HttpContext.RequestServices.GetService(typeof(IApplicationSitemapService)) as ApplicationSitemapService;

                applicationSitemapService.BearerToken = (User.Identity.IsAuthenticated ? await HttpContext.GetTokenAsync("id_token") : null); ;
                applicationSitemapService.RouteName = application.RouteName;
                applicationSitemapService.RootUrl = application.RootUrl;
                applicationSitemapService.SitemapUrl = application.SitemapUrl;
                applicationSitemapService.TheTask = applicationSitemapService.GetAsync();

                applicationSitemapServices.Add(applicationSitemapService);
            }

            // await all tasks to complete
            var allTasks = (from a in applicationSitemapServices select a.TheTask).ToArray();

            await Task.WhenAll(allTasks);

            // get the task results as individual sitemaps and merge into one
            var results = new Dto.Sitemap.Sitemap();

            foreach (var applicationSiteMap in applicationSitemapServices)
            {
                if (applicationSiteMap.TheTask.IsCompletedSuccessfully)
                {
                    var mappings = applicationSiteMap.TheTask.Result;

                    if (mappings.Count() > 0)
                    {
                        foreach (var mapping in mappings)
                        {
                            // rewrite the URL to swap the child application address for the composite UI address
                            mapping.Url = mapping.Url.Replace(applicationSiteMap.RootUrl, $"{baseUrl}/Application/Action?RouteName={applicationSiteMap.RouteName}&data=", StringComparison.InvariantCultureIgnoreCase);
                        }

                        results.AddRange(mappings);
                    }
                }
            }

            return results;
        }

    }
}