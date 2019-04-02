using System;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Prototype.Web.Web2.Data
{
    public class Trade : Models.BaseEditViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Title", Prompt = "Title", Description = "Enter the Title for this Trade")]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        public string Title { get; set; }

        [Display(Name = "Description", Prompt = "Description", Description = "Enter the Description for this Trade")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Minimum Age", Prompt = "Minimum Age", Description = "Enter the Minimum Age for this Trade")]
        public int MinimumAge { get; set; }

        [Display(Name = "City", Prompt = "City", Description = "Enter the City for this Trade")]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        public string City { get; set; }

        [Display(Name = "Category", Prompt = "Category", Description = "Enter the Category for this Trade")]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        public string Category { get; set; }
    }
}
