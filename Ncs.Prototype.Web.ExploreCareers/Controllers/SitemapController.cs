using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ncs.Prototype.Dto.Sitemap;
using Ncs.Prototype.Web.ExploreCareers.Services;

namespace Ncs.Prototype.Web.ExploreCareers.Controllers
{
    public class SitemapController : Controller
    {
        private readonly ILogger<SitemapController> _logger;
        private readonly ICareerService _careerService;

        public SitemapController(ILogger<SitemapController> logger, ICareerService careerService)
        {
            _logger = logger;
            _careerService = careerService;
        }

        [HttpGet]
        public async Task<ContentResult> Sitemap()
        {
            try
            {
                _logger.LogInformation("Generating Sitemap");

                const string careerControllerName = "Career";
                var sitemap = new Sitemap();

                // add the defaults
                sitemap.Add(new SitemapLocation() { Url = Url.Action(nameof(CareerController.Index), careerControllerName, null, Request.Scheme), Priority = 1 });

                string xmlString = sitemap.WriteSitemapToString();

                _logger.LogInformation("Generated Sitemap");

                return Content(xmlString, "application/xml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Sitemap)}: {ex.Message}");
            }

            return null;
        }
    }
}