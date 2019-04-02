using System;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Prototype.Web.ExploreCareers.Models
{
    public class Career: BaseEditViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name", Prompt = "Name", Description = "Enter the Name for this Skill")]
        [MaxLength(100, ErrorMessage = FieldLengthValidationError)]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        public string Name { get; set; }

        [Display(Name = "Description", Prompt = "Description", Description = "Enter a Description for this Skill")]
        [MaxLength(250, ErrorMessage = FieldLengthValidationError)]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Minimum Qualification", Prompt = "Minimum Qualification", Description = "Enter the Minimum Qualifications for this Skill")]
        [MaxLength(100, ErrorMessage = FieldLengthValidationError)]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        public string MinimumQualification { get; set; }

        [Display(Name = "City", Prompt = "City", Description = "Enter the City for this Skill")]
        [MaxLength(20, ErrorMessage = FieldLengthValidationError)]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        public string City { get; set; }
    }
}
