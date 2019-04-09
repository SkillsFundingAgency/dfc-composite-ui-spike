using System.ComponentModel.DataAnnotations;

namespace Ncs.Prototype.Web.Web1.Models
{
    public class SearchViewModel
    {
        [Display(Name = "Search Clue", Prompt = "Search Clue", Description = "Enter a Search Clue for a Course")]
        public string Clue { get; set; }
    }
}
