using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ncs.Prototype.Web.WebComposition.Controllers
{
    public class CompositeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Navbar()
        {
            return View();
        }
    }
}