using Newtonsoft.Json;
using System;

namespace Ncs.Prototype.Common
{
    public class ApplicationEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Name { get; set; }

        public DateTime Registered { get; set; }
        public DateTime? Modified { get; set; }

        public bool IsRegistered { get; set; }
        public bool IsOnline { get; set; }
        public bool IsHealthy { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string MainMenuText { get; set; }
        public string RouteName { get; set; }
        public string LayoutName { get; set; }
        public bool ShowSideBar { get; set; }
        public string Branding { get; set; }
        public bool RequiresAuthorization { get; set; }
        public string RootUrl { get; set; }
        public string SitemapUrl { get; set; }
        public string HealthCheckUrl { get; set; }
        public string EntrypointUrl { get; set; }
        public string SidebarUrl { get; set; }
        public string AppNavBarUrl { get; set; }
        public string BreadcrumbsUrl { get; set; }
        public string PersonalisationUrl { get; set; }
        public string BackButtonUrl { get; set; }
    }
}
