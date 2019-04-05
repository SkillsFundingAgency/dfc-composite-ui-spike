using Ncs.Prototype.Dto;
using System.Collections.Generic;

namespace Ncs.Prototype.Web.Composition.ViewComponents
{
    public class ApplicationNavigationMenuViewModel
    {
        public List<ApplicationDto> Applications { get; set; }
        public string SessionId { get; set; }
    }
}
