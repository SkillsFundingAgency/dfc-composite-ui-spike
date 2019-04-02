using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace Ncs.Prototype.Web.Web3.Models
{
    public class PartIndexViewModel
    {
        public IEnumerable<PartViewModel> Parts { get; set; }
        public IEnumerable<PartViewModel> PartsAuthorized { get; set; }
        public HtmlString HtmlParts { get; set; }
        public IEnumerable<PartViewModel> XmlParts { get; set; }
    }
}
