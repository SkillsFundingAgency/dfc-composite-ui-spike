using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Ncs.Prototype.Web.Web3.Services;

namespace Ncs.Prototype.Web.Web3.Controllers
{
    public class PartController : Controller
    {
        private readonly IPartService _PartService;

        public PartController(IPartService PartService)
        {
            _PartService = PartService;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            _PartService.BearerToken = (User.Identity.IsAuthenticated ? await HttpContext.GetTokenAsync("id_token") : null);

            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(_PartService.BearerToken))
            {
                var header = HttpContext.Request.Headers.FirstOrDefault(f => f.Key == "Authorization");

                if (header.Key != null)
                {
                    var parts = header.Value.First().Split(" ");

                    if (parts.Length > 1)
                    {
                        _PartService.BearerToken = parts[1];
                    }
                }
            }

            var vm = new Models.PartIndexViewModel();

            // create any required tasks
            var partsJsonTask = _PartService.GetPartsJsonAsync();
            var partsHtmlTask = _PartService.GetPartsHtmlAsync();
            var partsXmlTask = _PartService.GetPartsXmlAsync();
            Task<IEnumerable<Models.PartViewModel>> partsAuthorizedTask = null;

            var allTasks = new List<Task>() {partsJsonTask,partsHtmlTask,partsXmlTask};

            if (User.Identity.IsAuthenticated)
            {
                partsAuthorizedTask = _PartService.GetPartsAuthorizedAsync();

                allTasks.Add(partsAuthorizedTask);
            }

            // await all tasks to complete
            await Task.WhenAll(allTasks.ToArray());

            // get the task results as markup
            vm.Parts = partsJsonTask.Result;
            vm.HtmlParts = partsHtmlTask.Result;
            vm.XmlParts = partsXmlTask.Result;

            if (User.Identity.IsAuthenticated)
            {
                vm.PartsAuthorized = partsAuthorizedTask.Result;
            }

            return View(vm);
        }
    }
}