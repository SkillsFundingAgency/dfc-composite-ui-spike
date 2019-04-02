using Microsoft.AspNetCore.Html;

namespace Ncs.Prototype.Web.WebComposition.Models
{
    public class PageViewModel
    {
        public string LayoutName { get; set; } = "_Layout";
        public string Branding { get; set; } = "ESFA";
        public string PageTitle { get; set; } = "Unknown Service";

        public HtmlString AppNavBarMarkup { get; set; }
        public HtmlString BreadcrumbsMarkup { get; set; }
        public HtmlString PersonalisationMarkup { get; set; }
        public HtmlString BackButtonMarkup { get; set; }
        public HtmlString ApplicationMarkup { get; set; }
        public HtmlString SidebarMarkup { get; set; }
    }
}
