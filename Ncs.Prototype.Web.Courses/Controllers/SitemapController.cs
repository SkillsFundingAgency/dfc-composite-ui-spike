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

                string baseUrl = BaseUrl();
                var sitemap = new Sitemap();

                sitemap.Add(new SitemapLocation() { Url = $"{baseUrl}/Course", Priority = 1 });

                var filters = new List<string> { "Mine", "ThisMonth", "NextMonth" };
                filters.ForEach(f => sitemap.Add(new SitemapLocation() { Url = $"{baseUrl}/Course?filter={f}", Priority = 1 }));

                var categories = GetCourseCategories();

                categories.ForEach(c => sitemap.Add(new SitemapLocation() { Url = $"{baseUrl}/Course?category={c}", Priority = 1 }));

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

        private string BaseUrl()
        {
            return string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Content("~"));
        }

        private List<string> GetCourseCategories()
        {
            var courseService = HttpContext.RequestServices.GetService(typeof(ICourseService)) as CourseService;
            var courses = courseService.GetCourses();

            var categories = courses.GroupBy(g => g.Category)
                .Select(s => s.Key)
                .OrderBy(o => o)
                .ToList();

            return categories;
        }
    }
}