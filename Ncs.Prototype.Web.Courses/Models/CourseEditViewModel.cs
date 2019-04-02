using System;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Prototype.Web.Courses.Models
{
    public class CourseEditViewModel: BaseEditViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Title", Prompt = "Title", Description = "Enter the Title for this Course")]
        public string Title { get; set; }

        [Display(Name = "Description", Prompt = "Description", Description = "Enter the Description for this Course")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Start Date", Prompt = "Start Date", Description = "Enter the Start date for this Course")]
        public DateTime Start { get; set; }

        [Display(Name = "Max Attendees", Prompt = "Max Attendees", Description = "Enter the Maximum number of attendees for this Course")]
        public int MaxAttendeeCount { get; set; }

        [Display(Name = "City", Prompt = "City", Description = "Enter the City for this Course")]
        public string City { get; set; }
    }
}
