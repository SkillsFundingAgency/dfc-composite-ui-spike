using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor.FeedProcessing;
using Microsoft.Extensions.Caching.Memory;
using Ncs.Prototype.Common;
using Ncs.Prototype.Web.Composition.Loggers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.Composition.ChangeFeedServices
{
    public class CosmosDBObserver : IChangeFeedObserver
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILog _log;

        public CosmosDBObserver(IMemoryCache memoryCache, ILog log)
        {
            _memoryCache = memoryCache;
            _log = log;
        }

        public Task CloseAsync(IChangeFeedObserverContext context, ChangeFeedObserverCloseReason reason)
        {
            Log("CloseAsync." + reason.ToString());
            return Task.CompletedTask;
        }

        public Task OpenAsync(IChangeFeedObserverContext context)
        {
            Log("OpenAsync." + context.PartitionKeyRangeId);
            return Task.CompletedTask;
        }

        public Task ProcessChangesAsync(IChangeFeedObserverContext context, IReadOnlyList<Document> docs, CancellationToken cancellationToken)
        {
            Log("ProcessChangesAsync");

            if (docs != null && docs.Count > 0)
            {
                Log($"ProcessChangesAsync for {docs.Count} documents. Id of first is {docs[0].Id}");
            }
            else
            {
                Log($"ProcessChangesAsync for 0 documents");
            }
            
            _memoryCache.Remove(CacheKey.Applications);
            
            return Task.CompletedTask;
        }

        private void Log(string data)
        {
            _log.Log($"CosmosDBObserver {data}");
        }

    }
}
