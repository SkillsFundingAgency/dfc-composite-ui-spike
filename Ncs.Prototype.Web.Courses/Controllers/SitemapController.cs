using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ncs.Prototype.Dto.Sitemap;
using Ncs.Prototype.Web.Courses.Services;

namespace Ncs.Prototype.Web.Courses.Controllers
{
    public class SitemapController : Controller
    {
        private readonly ILogger<SitemapController> _logger;
        private readonly ICourseService _courseService;

        public SitemapController(ILogger<SitemapController> logger, ICourseService courseService)
        {
            _logger = logger;
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ContentResult> Sitemap()
        {
            try
            {
                _logger.LogInformation("Generating Sitemap");

                const string courseControllerName = "Course";
                var sitemap = new Sitemap();

                // add the defaults
                sitemap.Add(new SitemapLocation() { Url = Url.Action(nameof(CourseController.Index), courseControllerName, null, Request.Scheme), Priority = 1 });
                sitemap.Add(new SitemapLocation() { Url = Url.Action(nameof(CourseController.Search), courseControllerName, null, Request.Scheme), Priority = 1 });

                // add the filters
                CourseController.Filters.ForEach(f => sitemap.Add(new SitemapLocation() { Url = Url.Action(nameof(CourseController.Index), courseControllerName, new { Filter = f }, Request.Scheme), Priority = 1 }));

                // add the categories
                var categories = GetCourseCategories();

                categories.ForEach(c => sitemap.Add(new SitemapLocation() { Url = Url.Action(nameof(CourseController.Index), courseControllerName, new { Category = c }, Request.Scheme), Priority = 1 }));

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

        private List<string> GetCourseCategories()
        {
            var categories = _courseService.GetCategories();

            var results = categories.Select(s => s.Name)
                                    .OrderBy(o => o)
                                    .ToList();
            return results;
        }
    }
}