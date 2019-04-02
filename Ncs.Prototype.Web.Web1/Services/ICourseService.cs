using System.Collections.Generic;
using Ncs.Prototype.Web.Web1.Data;

namespace Ncs.Prototype.Web.Web1.Services
{
    public interface ICourseService
    {
        Course GetCourse(int id);
        List<Course> GetCourses(string city = null, string category = null, bool filterThisMonth = false, bool filterNextMonth = false);
    }
}