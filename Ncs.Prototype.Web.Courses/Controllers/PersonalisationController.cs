using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ncs.Prototype.Web.Courses.Controllers
{
    [Authorize]
    public class PersonalisationController : Controller
    {
        [HttpGet]
        public IActionResult Welcome()
        {
            return View();
        }

    }
}