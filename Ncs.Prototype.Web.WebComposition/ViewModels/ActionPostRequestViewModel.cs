using Microsoft.AspNetCore.Http;

namespace Ncs.Prototype.Web.WebComposition.ViewModels
{
    public class ActionPostRequestViewModel
    {
        public string ApplicationName { get; set; }
        public IFormCollection FormCollection { get; set; }
    }
}
