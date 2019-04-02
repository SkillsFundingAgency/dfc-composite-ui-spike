using System;
using System.Collections.Generic;
using System.Linq;
using Ncs.Prototype.Web.ExploreCareers.Models;

namespace Ncs.Prototype.Web.ExploreCareers.Services
{
    public class CareerService : ICareerService
    {
        private List<Career> _Careers;

        public CareerService()
        {
            _Careers = new List<Models.Career>()
            {
                 new Models.Career()
                 {
                     Id=1,
                     Name="Archivist",
                     Description="Archivists look after and preserve collections of historical records and documents.",
                     MinimumQualification="2 to 3 A levels for a degree",
                     City="Banbury"
                 },
                 new Models.Career()
                 {
                     Id=2,
                     Name="Business analyst",
                     Description="Business analysts work with organisations to help them change and improve.",
                     MinimumQualification="2 to 3 A levels for a degree",
                     City="Solihull"
                 },
                 new Models.Career()
                 {
                     Id=3,
                     Name="Cartographer",
                     Description="Cartographers collect information about the geography of an area to design and produce maps, charts and plans.",
                     MinimumQualification="a degree in a relevant subject for postgraduate study",
                     City="Banbury"
                 },
                 new Models.Career()
                 {
                     Id=4,
                     Name="Computer games developer",
                     Description="Computer games developers make games for the internet, mobile phones, PCs and games consoles.",
                     MinimumQualification="2 to 3 A levels for a degree",
                     City="Solihull"
                 },
                 new Models.Career()
                 {
                     Id=5,
                     Name="Computer games tester",
                     Description="Computer games testers play computer games to check they work, and find and record problems or ‘bugs’.",
                     MinimumQualification="1 or 2 A levels for a foundation degree or higher national diploma",
                     City="Banbury"
                 },
                 new Models.Career()
                 {
                     Id=6,
                     Name="Cyber intelligence officer",
                     Description="Cyber intelligence officers gather information about where threats to information technology (IT) systems come from and how they work.",
                     MinimumQualification="a degree in any subject for a postgraduate course",
                     City="Solihull"
                 },
                 new Models.Career()
                 {
                     Id=7,
                     Name="Data entry clerk",
                     Description="Data entry clerks type information into databases and systems and create letters, reports and other documents.",
                     MinimumQualification="Level 2 European Computer Driving Licence (ECDL) Certificate in IT User Career",
                     City="Banbury"
                 },
                 new Models.Career()
                 {
                     Id=8,
                     Name="Database administrator",
                     Description="Database administrators (DBAs) plan and build computer systems, and make sure they're secure and working properly.",
                     MinimumQualification="1 or 2 A levels for a higher national diploma",
                     City="Banbury"
                 },
                 new Models.Career()
                 {
                     Id=9,
                     Name="Digital delivery manager",
                     Description="Digital delivery managers are responsible for the performance of a team and the digital products and services they produce.",
                     MinimumQualification="a degree in a relevant subject for postgraduate study",
                     City="Solihull"
                 },
                 new Models.Career()
                 {
                     Id=10,
                     Name="Digital marketer",
                     Description="Digital marketers promote brands, products and services through social media, websites and apps.",
                     MinimumQualification="2 to 3 A levels to do a degree or higher national diploma",
                     City="Solihull"
                 },
             };
      }


        public List<Career> GetCareers(string city = null)
        {
            var results = _Careers;

            if (!string.IsNullOrEmpty(city))
            {
                results = results.Where(x => x.City == city).ToList();
            }

            return results;
        }

        public Career GetCareer(int id)
        {
            return _Careers.FirstOrDefault(x => x.Id == id);
        }
    }
}
