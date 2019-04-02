using System.Collections.Generic;
using Ncs.Prototype.Web.ExploreCareers.Models;

namespace Ncs.Prototype.Web.ExploreCareers.Services
{
    public interface ICareerService
    {
        Career GetCareer(int id);
        List<Career> GetCareers(string city = null);
    }
}