using System.Collections.Generic;
using System.Threading.Tasks;
using Ncs.Prototype.Dto.Sitemap;

namespace Ncs.Prototype.Web.Composition.Services
{
    public interface IApplicationSitemapService
    {
        string BearerToken { get; set; }
        string RouteName { get; set; }
        string RootUrl { get; set; }
        string SitemapUrl { get; set; }
        Task<IEnumerable<SitemapLocation>> TheTask { get; set; }

        Task<IEnumerable<SitemapLocation>> GetAsync();
    }
}