using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ncs.Prototype.Common;
using Ncs.Prototype.Dto;
using TestWebApplication.Models;

namespace TestWebApplication.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILog _log;
        private readonly IMemoryCache _memoryCache;
        public HomeController(ILog log, IMemoryCache memoryCache, IHostingEnvironment hostingEnvironment)
        {
            _log = log;
            _log.Log(hostingEnvironment.EnvironmentName);
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            var applications = new List<ApplicationDto>();
            _memoryCache.TryGetValue(CacheKey.Applications, out applications);
            ViewData["Applications"] = applications;
            _log.Log("Inside HomeController.Index");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
