using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Ncs.Prototype.Web.SkillsHealthCheck.Controllers
{
    public class SkillController : Controller
    {
        private readonly Models.SkillsViewModel Skills = new Models.SkillsViewModel()
        {
            Skills = new List<Models.SkillViewModel>()
             {
                 new Models.SkillViewModel()
                 {
                     Id=1,
                     Name="Archivist",
                     Description="Archivists look after and preserve collections of historical records and documents.",
                     MinimumQualification="2 to 3 A levels for a degree"
                 },
                 new Models.SkillViewModel()
                 {
                     Id=2,
                     Name="Business analyst",
                     Description="Business analysts work with organisations to help them change and improve.",
                     MinimumQualification="2 to 3 A levels for a degree"
                 },
                 new Models.SkillViewModel()
                 {
                     Id=3,
                     Name="Cartographer",
                     Description="Cartographers collect information about the geography of an area to design and produce maps, charts and plans.",
                     MinimumQualification="a degree in a relevant subject for postgraduate study"
                 },
                 new Models.SkillViewModel()
                 {
                     Id=4,
                     Name="Computer games developer",
                     Description="Computer games developers make games for the internet, mobile phones, PCs and games consoles.",
                     MinimumQualification="2 to 3 A levels for a degree"
                 },
                 new Models.SkillViewModel()
                 {
                     Id=5,
                     Name="Computer games tester",
                     Description="Computer games testers play computer games to check they work, and find and record problems or ‘bugs’.",
                     MinimumQualification="1 or 2 A levels for a foundation degree or higher national diploma"
                 },
                 new Models.SkillViewModel()
                 {
                     Id=6,
                     Name="Cyber intelligence officer",
                     Description="Cyber intelligence officers gather information about where threats to information technology (IT) systems come from and how they work.",
                     MinimumQualification="a degree in any subject for a postgraduate course"
                 },
                 new Models.SkillViewModel()
                 {
                     Id=7,
                     Name="Data entry clerk",
                     Description="Data entry clerks type information into databases and systems and create letters, reports and other documents.",
                     MinimumQualification="Level 2 European Computer Driving Licence (ECDL) Certificate in IT User Skills"
                 },
                 new Models.SkillViewModel()
                 {
                     Id=8,
                     Name="Database administrator",
                     Description="Database administrators (DBAs) plan and build computer systems, and make sure they’re secure and working properly.",
                     MinimumQualification="1 or 2 A levels for a higher national diploma"
                 },
                 new Models.SkillViewModel()
                 {
                     Id=9,
                     Name="Digital delivery manager",
                     Description="Digital delivery managers are responsible for the performance of a team and the digital products and services they produce.",
                     MinimumQualification="a degree in a relevant subject for postgraduate study"
                 },
                 new Models.SkillViewModel()
                 {
                     Id=10,
                     Name="Digital marketer",
                     Description="Digital marketers promote brands, products and services through social media, websites and apps.",
                     MinimumQualification="2 to 3 A levels to do a degree or higher national diploma"
                 },
             }
        };

        // GET: Explore/Index
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            return View(Skills);
        }

        // GET: Explore/Edit/5
        public async System.Threading.Tasks.Task<ActionResult> Edit(int id)
        {
            var viewModel = Skills.Skills.FirstOrDefault(f => f.Id == id);

            return View(viewModel);
        }

        // POST: Explore/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Edit(Models.SkillViewModel SkillViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var itemToUpdate = Skills.Skills.FirstOrDefault(f => f.Id == SkillViewModel.Id);

                    if (itemToUpdate == null)
                    {
                        ModelState.AddModelError(string.Empty, $"Unable to find model for id: {SkillViewModel.Id}");
                    }
                    else
                    {
                        itemToUpdate.Name = SkillViewModel.Name;
                        itemToUpdate.Description = SkillViewModel.Description;
                        itemToUpdate.MinimumQualification = SkillViewModel.MinimumQualification;

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error in request: {ex.Message}");
            }

            return View(SkillViewModel);
        }

    }
}