using Ncs.Prototype.Web.Courses.Data;
using System.Collections.Generic;

namespace Ncs.Prototype.Web.Courses.Models
{
    public class SidebarViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
    }
}
