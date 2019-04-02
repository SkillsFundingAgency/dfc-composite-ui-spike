using Microsoft.Azure.Documents.ChangeFeedProcessor.FeedProcessing;
using Microsoft.Extensions.Caching.Memory;

namespace TestWebApplication.ChangeFeedServices
{
    public class CosmosDBObserverFactory : IChangeFeedObserverFactory
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILog _log;

        public CosmosDBObserverFactory(IMemoryCache memoryCache, ILog log)
        {
            _memoryCache = memoryCache;
            _log = log;
        }

        public IChangeFeedObserver CreateObserver()
        {
            Log("CreateObserver");

            return new CosmosDBObserver(_memoryCache, _log);
        }

        private void Log(string data)
        {
            _log.Log($"CosmosDBObserverFactory {data}");
        }
    }
}
