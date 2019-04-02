using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ncs.Prototype.Web.Composition.Framework
{
    public class CorrelationIdDelegatingHandler : DelegatingHandler
    {
        private readonly ICorrelationContextAccessor correlationContextAccessor;
        private readonly IOptions<CorrelationIdOptions> options;
        private readonly ILogger<CorrelationIdDelegatingHandler> logger;

        public CorrelationIdDelegatingHandler(
            ICorrelationContextAccessor correlationContextAccessor,
            IOptions<CorrelationIdOptions> options,
            ILogger<CorrelationIdDelegatingHandler> logger
)
        {
            this.correlationContextAccessor = correlationContextAccessor;
            this.options = options;
            this.logger = logger;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(this.options.Value.Header))
            {
                request.Headers.Add(this.options.Value.Header, correlationContextAccessor.CorrelationContext.CorrelationId);
                logger.Log(LogLevel.Information, $"Added CorrelationID: {correlationContextAccessor.CorrelationContext.CorrelationId}");
            }

            // Else the header has already been added due to a retry.

            return base.SendAsync(request, cancellationToken);
        }
    }
}
