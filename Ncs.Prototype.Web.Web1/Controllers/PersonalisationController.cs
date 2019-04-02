using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ncs.Prototype.Web.Web1.Controllers
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