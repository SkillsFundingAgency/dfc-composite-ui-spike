using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestWebApplication.ChangeFeedServices
{
    public class CosmosDBHostedBackgroundService : BackgroundService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILog _log;
        private readonly CosmosSettings _cosmosSettings;
        public CosmosDBHostedBackgroundService(IMemoryCache memoryCache, ILog log, CosmosSettings cosmosSettings)
        {
            _log = log;
            _cosmosSettings = cosmosSettings;
            Log("Constructor");
            _memoryCache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Log("ExecuteAsync");

                var hostName = _cosmosSettings.HostName;
                var dbName = _cosmosSettings.DatabaseName;
                var key = _cosmosSettings.Key;
                var uri = _cosmosSettings.Uri;

                DocumentCollectionInfo feedCollectionInfo = new DocumentCollectionInfo()
                {
                    DatabaseName = dbName,
                    CollectionName = _cosmosSettings.CollectionName,
                    Uri = new Uri(uri),
                    MasterKey = key
                };

                DocumentCollectionInfo leaseCollectionInfo = new DocumentCollectionInfo()
                {
                    DatabaseName = dbName,
                    CollectionName = _cosmosSettings.LeaseCollectionName,
                    Uri = new Uri(uri),
                    MasterKey = key
                };

                var observerFactory = new CosmosDBObserverFactory(_memoryCache, _log);
                var builder = new ChangeFeedProcessorBuilder();
                var processor = await builder
                    .WithHostName(hostName)
                    .WithFeedCollection(feedCollectionInfo)
                    .WithLeaseCollection(leaseCollectionInfo)
                    .WithObserverFactory(observerFactory)
                    .BuildAsync();

                Log("before StartAsync");
                await processor.StartAsync();
                Log("after StartAsync");

                await Task.Delay(-1);
                Console.WriteLine("Change Feed Processor started. Press <Enter> key to stop...");
                Console.ReadLine();
                
                Log("before StopAsync");
                await processor.StopAsync();
                Log("after StopAsync");
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private void Log(string data)
        {
            _log.Log($"CosmosDBHostedBackgroundService {data}");
        }

    }
}
