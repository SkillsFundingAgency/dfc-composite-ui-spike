using System.ComponentModel.DataAnnotations;

namespace Ncs.Prototype.Web.Web3.Models
{
    public class PartViewModel : BaseEditViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name", Prompt = "Name", Description = "Enter the Name for this Part")]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        public string Name { get; set; }

        [Display(Name = "Description", Prompt = "Description", Description = "Enter the Description for this Part")]
        [Required(ErrorMessage = FieldMandatoryhValidationError)]
        public string Description { get; set; }
    }
}
