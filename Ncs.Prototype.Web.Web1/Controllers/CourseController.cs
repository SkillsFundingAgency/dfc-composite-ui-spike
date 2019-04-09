using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Ncs.Prototype.Web.Web1.Data;
using Ncs.Prototype.Web.Web1.Models;
using Ncs.Prototype.Web.Web1.Services;

namespace Ncs.Prototype.Web.Web1.Controllers
{
    public class CourseController : Controller
    {
        public const string FilterMine = "Mine";
        public const string FilterThisMonth = "ThisMonth";
        public const string FilterNextMonth = "NextMonth";
        public static readonly List<string> Filters = new List<string> { FilterMine, FilterThisMonth, FilterNextMonth };

        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public IActionResult Index(string category, string filter, string searchClue)
        {
            var vm = new CourseIndexViewModel();
            string city = ((string.Compare(filter, FilterMine, true) == 0) ? GetCity() : string.Empty);
            bool filterThisMonth = (string.Compare(filter, FilterThisMonth, true) == 0);
            bool filterNextMonth = (string.Compare(filter, FilterNextMonth, true) == 0);

            vm.Courses = _courseService.GetCourses(city, category, filterThisMonth, filterNextMonth, searchClue);

            return View(vm);
        }

        [HttpGet]
        public IActionResult Sidebar()
        {
            var vm = new SidebarViewModel();
            var courses = _courseService.GetCourses();

            vm.Categories = _courseService.GetCategories();

            return View(vm);
        }

        [HttpGet]
        public IActionResult Search(string searchClue)
        {
            if (!string.IsNullOrEmpty(searchClue))
            {
                return RedirectToAction(nameof(Index), new { searchClue });
            }

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