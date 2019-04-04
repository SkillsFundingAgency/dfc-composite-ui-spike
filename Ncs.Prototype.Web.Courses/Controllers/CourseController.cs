using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ncs.Prototype.Web.Courses.Data;
using Ncs.Prototype.Web.Courses.Models;
using Ncs.Prototype.Web.Courses.Services;

namespace Ncs.Prototype.Web.Courses.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public IActionResult Index(string category, string filter, string searchClue)
        {
            var key = "Visits";
            var visits = HttpContext.Session.GetInt32(key) ?? 0;
            visits += 1;
            HttpContext.Session.SetInt32(key, visits);
            ViewData[key] = visits;

            var vm = new CourseIndexViewModel();
            string city = ((string.Compare(filter, "Mine", true) == 0) ? GetCity() : string.Empty);
            bool filterThisMonth = (string.Compare(filter, "ThisMonth", true) == 0);
            bool filterNextMonth = (string.Compare(filter, "NextMonth", true) == 0);

            vm.Courses = _courseService.GetCourses(city, category, filterThisMonth, filterNextMonth, searchClue);

            if (DateTime.Now.Second >= 45)
            {
                throw new System.Exception("kaboom");
            }
            if (DateTime.Now.Second > 10 && DateTime.Now.Second < 15)
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, 13));
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Sidebar()
        {
            var vm = new SidebarViewModel();
            var courses = _courseService.GetCourses();

            var categories = courses.GroupBy(g => g.Category)
                .Select(s => new Data.Category()
                {
                    Name = s.Key,
                    CourseCount = s.Count()
                }
                )
                .OrderBy(o => o.Name)
                .ToList();

            vm.Categories = categories;

            return View(vm);
        }

        [HttpGet]
        public IActionResult Search()
        {
            var vm = new SearchViewModel();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(SearchViewModel search)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(search.Clue))
                {
                    return RedirectToAction(nameof(Index), new { searchClue = search.Clue });
                }
            }

            return View(search);
        }

        [HttpGet]
        public IActionResult Navbar()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var vm = _courseService.GetCourse(id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                bool isAuthenticated = User.Identity.IsAuthenticated;

                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }

        private string GetCity()
        {
            var result = string.Empty;
            var emailAddress = string.Empty;

            if (User.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                emailAddress = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            }

            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                emailAddress = emailAddress.ToLower();
                if (emailAddress == "ilyasdhin@gmail.com")
                {
                    result = "Solihull";
                }
                else if (emailAddress == "ianshangout99@gmail.com")
                {
                    result = "Coventry";
                }
            }
            return result;
        }
    }
}