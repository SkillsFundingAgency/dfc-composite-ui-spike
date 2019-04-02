using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.SkillsHealthCheck.Models
{
    public class SkillsViewModel
    {
        [Display(Name = "Skills", Description = "Skills", Prompt = "Skills")]
        public IList<SkillViewModel> Skills { get; set; }
    }
}
