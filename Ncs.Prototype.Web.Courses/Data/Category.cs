using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.Courses.Data
{
    public class Category : Models.BaseEditViewModel
    {
        [Display(Name = "Category", Prompt = "Category", Description = "Enter the Category for this Course")]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        public string Name { get; set; }

        [Display(Name = "Courses", Prompt = "Courses", Description = "Enter the number of courses for this Category")]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        public int CourseCount { get; set; }
    }
}
