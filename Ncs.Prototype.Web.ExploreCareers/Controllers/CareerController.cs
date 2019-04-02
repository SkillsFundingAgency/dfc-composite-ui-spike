using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Ncs.Prototype.Web.ExploreCareers.Models;
using Ncs.Prototype.Web.ExploreCareers.Services;

namespace Ncs.Prototype.Web.ExploreCareers.Controllers
{
    public class CareerController : Controller
    {
        private readonly ICareerService _CareerService;

        public CareerController(ICareerService CareerService)
        {
            _CareerService = CareerService;
        }

        [HttpGet]
        public IActionResult Index(string category, string filter)
        {
            var vm = new CareerIndexViewModel();
            string city = GetCity();

            vm.Careers = _CareerService.GetCareers(city);

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
            return View();
        }

        [HttpGet]
        public IActionResult Navbar()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var vm = _CareerService.GetCareer(id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Career Career)
        {
            if (ModelState.IsValid)
            {
                bool isAuthenticated = User.Identity.IsAuthenticated;

                return RedirectToAction(nameof(Index));
            }

            return View(Career);
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
                    result = "Banbury";
                }
            }
            return result;
        }
    }
}