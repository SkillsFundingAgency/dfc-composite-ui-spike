using Microsoft.AspNetCore.Http;

namespace Ncs.Prototype.Web.Composition.ViewModels
{
    public class ActionPostRequestViewModel
    {
        public string RouteName { get; set; }
        public IFormCollection FormCollection { get; set; }
    }
}
