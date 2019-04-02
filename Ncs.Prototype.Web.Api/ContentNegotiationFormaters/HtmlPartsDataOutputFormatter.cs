using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Ncs.Prototype.Web.Api.ContentNegotiationFormaters
{
    public class HtmlPartsDataOutputFormatter: TextOutputFormatter
    {
        public HtmlPartsDataOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/html"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            if (typeof(Models.PartsData).IsAssignableFrom(type)
                || typeof(IEnumerable<Models.PartsData>).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }
            return false;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var logger = serviceProvider.GetService(typeof(ILogger<HtmlPartsDataOutputFormatter>)) as ILogger;

            var response = context.HttpContext.Response;

            var buffer = new StringBuilder();

            buffer.Append("<table class='govuk-table'>");
            buffer.Append("<caption class='govuk-table__caption'>Parts List</caption>");

            buffer.Append("<thead class='govuk-table__head'>");
            buffer.Append("<tr class='govuk-table__row'>");
            buffer.Append("<th class='govuk-table__header' scope='col'>");
            buffer.Append($"{nameof(Models.PartsData.Name)}");
            buffer.Append("</th>");
            buffer.Append("<th class='govuk-table__header' scope='col'>");
            buffer.Append($"{nameof(Models.PartsData.Description)}");
            buffer.Append("</th>");
            buffer.Append("</tr>");
            buffer.Append("</thead>");

            buffer.Append("<tbody class='govuk-table__body'>");

            if (context.Object is IEnumerable<Models.PartsData>)
            {
                foreach (Models.PartsData partsData in context.Object as IEnumerable<Models.PartsData>)
                {
                    FormatPartsData(buffer, partsData, logger);
                }
            }
            else
            {
                var partsData = context.Object as Models.PartsData;

                FormatPartsData(buffer, partsData, logger);
            }

            buffer.Append("</tbody>");
            buffer.Append("</table>");

            return response.WriteAsync(buffer.ToString());
        }

        private static void FormatPartsData(StringBuilder buffer, Models.PartsData partsData, ILogger logger)
        {
            buffer.Append("<tr class='govuk-table__row'>");
            buffer.Append("<td class='govuk-table__cell'>");
            buffer.Append($"<span>{partsData.Name}</span>");
            buffer.Append("</td>");
            buffer.Append("<td class='govuk-table__cell'>");
            buffer.Append($"<span>{partsData.Description}</span>");
            buffer.Append("</td>");
            buffer.Append("</tr>");

            logger.LogInformation($"Writing {partsData.Name} - {partsData.Description}");
        }

    }
}
