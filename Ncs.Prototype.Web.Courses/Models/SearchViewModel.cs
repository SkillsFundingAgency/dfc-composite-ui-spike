using Ncs.Prototype.Web.Courses.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Prototype.Web.Courses.Models
{
    public class SearchViewModel
    {
        [Display(Name = "Search Clue", Prompt = "Search Clue", Description = "Enter a Search Clue for a Course")]
        public string Clue { get; set; }
    }
}
