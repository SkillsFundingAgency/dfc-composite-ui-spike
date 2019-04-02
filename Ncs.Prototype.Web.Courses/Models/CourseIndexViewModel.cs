using Ncs.Prototype.Web.Courses.Data;
using System.Collections.Generic;

namespace Ncs.Prototype.Web.Courses.Models
{
    public class CourseIndexViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
    }
}
