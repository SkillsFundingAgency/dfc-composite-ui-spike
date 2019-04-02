using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Ncs.Prototype.Web.Web3.Models;

namespace Ncs.Prototype.Web.Web3.Services
{
    public interface IPartService
    {
        string BearerToken { get; set; }

        Task<IEnumerable<PartViewModel>> GetPartsJsonAsync();
        Task<IEnumerable<PartViewModel>> GetPartsAuthorizedAsync();
        Task<HtmlString> GetPartsHtmlAsync();
        Task<IEnumerable<PartViewModel>> GetPartsXmlAsync();
    }
}