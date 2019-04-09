using System.Collections.Generic;
using System.Threading.Tasks;
using Ncs.Prototype.Dto.Sitemap;

namespace Ncs.Prototype.Web.WebComposition.Services
{
    public interface IApplicationSitemapService
    {
        string BearerToken { get; set; }
        string SitemapUrl { get; set; }
        Task<IEnumerable<SitemapLocation>> TheTask { get; set; }

        Task<IEnumerable<SitemapLocation>> GetAsync();
    }
}