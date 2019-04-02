using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Ncs.Prototype.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.ApplicationManagement.Services
{
    public class CosmosDbStorage : IStorage
    {
        private readonly string _endpointUri;
        private readonly string _key;

        public CosmosDbStorage(string endpointUri, string key)
        {
            _endpointUri = endpointUri;
            _key = key;
        }

        public async Task Add<T>(string databaseId, string collectionId, T document)
        {
            var client = await Init(databaseId, collectionId);

            try
            {
                await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), document);
            }
            catch (DocumentClientException dex)
            {
                if (dex.StatusCode != HttpStatusCode.Conflict)
                {
                    throw;
                }
            }
        }

        public async Task<T> Get<T>(string databaseId, string collectionId, string documentId)
        {
            var client = await Init(databaseId, collectionId);

            var link = UriFactory.CreateDocumentUri(databaseId, collectionId, documentId);
            var readResponse = await client.ReadDocumentAsync<T>(link);

            return readResponse.Document;
        }

        public async Task<List<T>> Search<T>(string databaseId, string collectionId, Expression<Func<T, bool>> expression)
        {
            var client = await Init(databaseId, collectionId);

            var queryOptions = new FeedOptions { MaxItemCount = int.MaxValue };

            IDocumentQuery<T> query = null;
            if (expression != null)
            {
                query = client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), queryOptions)
                    .Where(expression)
                    .AsDocumentQuery();
            }
            else
            {
                query = client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), queryOptions)
                    .AsDocumentQuery();
            }

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task Update<T>(string databaseId, string collectionId, string documentId, T document)
        {
            var client = await Init(databaseId, collectionId);

            var link = UriFactory.CreateDocumentUri(databaseId, collectionId, documentId);
            await client.ReplaceDocumentAsync(link, document);
        }

        public async Task Delete(string databaseId, string collectionId, string documentId)
        {
            var client = await Init(databaseId, collectionId);

            var link = UriFactory.CreateDocumentUri(databaseId, collectionId, documentId);
            await client.DeleteDocumentAsync(link);
        }

        private async Task<DocumentClient> Init(string dbId, string collectionId)
        {
            var db = new Database();
            db.Id = dbId;

            //create db
            var client = new DocumentClient(new Uri(_endpointUri), _key);
            await client.CreateDatabaseIfNotExistsAsync(db);

            //create document collection
            var docCollection = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(dbId),
                new DocumentCollection { Id = collectionId });

            return client;
        }
    }
}
