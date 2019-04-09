using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ncs.Prototype.Dto.Sitemap;
using Ncs.Prototype.Web.Web3.Services;

namespace Ncs.Prototype.Web.Web3.Controllers
{
    public class SitemapController : Controller
    {
        private readonly ILogger<SitemapController> _logger;
        private readonly IPartService _PartService;

        public SitemapController(ILogger<SitemapController> logger, IPartService PartService)
        {
            _logger = logger;
            _PartService = PartService;
        }

        [HttpGet]
        public async Task<ContentResult> Sitemap()
        {
            try
            {
                _logger.LogInformation("Generating Sitemap");

                const string PartControllerName = "Part";
                var sitemap = new Sitemap();

                // add the defaults
                sitemap.Add(new SitemapLocation() { Url = Url.Action(nameof(PartController.Index), PartControllerName, null, Request.Scheme), Priority = 1 });

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