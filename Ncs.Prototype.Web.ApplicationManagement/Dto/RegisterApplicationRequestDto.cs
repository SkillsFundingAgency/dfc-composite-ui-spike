namespace Ncs.Prototype.Web.ApplicationManagement.Dto
{
    public class RegisterApplicationRequestDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MainMenuText { get; set; }
        public string RouteName { get; set; }
        public string LayoutName { get; set; }
        public bool ShowSideBar { get; set; }
        public string Branding { get; set; }
        public bool RequiresAuthorization { get; set; }
        public string RootUrl { get; set; }
        public string HealthCheckUrl { get; set; }
        public string EntrypointUrl { get; set; }
        public string SidebarUrl { get; set; }
        public string AppNavBarUrl { get; set; }
        public string BreadcrumbsUrl { get; set; }
        public string PersonalisationUrl { get; set; }
        public string BackButtonUrl { get; set; }
    }
}
