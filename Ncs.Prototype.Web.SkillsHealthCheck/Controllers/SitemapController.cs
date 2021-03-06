﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ncs.Prototype.Dto.Sitemap;

namespace Ncs.Prototype.Web.SkillsHealthCheck.Controllers
{
    public class SitemapController : Controller
    {
        private readonly ILogger<SitemapController> _logger;

        public SitemapController(ILogger<SitemapController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ContentResult> Sitemap()
        {
            try
            {
                _logger.LogInformation("Generating Sitemap");

                const string skillControllerName = "Skill";
                var sitemap = new Sitemap();

                // add the defaults
                sitemap.Add(new SitemapLocation() { Url = Url.Action(nameof(SkillController.Index), skillControllerName, null, Request.Scheme), Priority = 1 });

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