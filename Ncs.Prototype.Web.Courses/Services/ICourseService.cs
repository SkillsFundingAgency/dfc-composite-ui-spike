using System.Collections.Generic;
using Ncs.Prototype.Web.Courses.Data;

namespace Ncs.Prototype.Web.Courses.Services
{
    public interface ICourseService
    {
        Course GetCourse(int id);
        List<Course> GetCourses(string city = null, string category = null, bool filterThisMonth = false, bool filterNextMonth = false, string searchClue = null);
    }
}