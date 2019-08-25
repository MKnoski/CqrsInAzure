using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace CqrsInAzure.Common
{
    public abstract class DocumentDbRepository<T>
        where T : class
    {
        protected readonly DocumentClient Client;

        protected readonly string CollectionId = "";
        protected readonly string DatabaseId = "";
        protected readonly string Endpoint = "";
        protected readonly string AuthKey = "";
        protected readonly string PartitionKeyPath = "";

        protected DocumentDbRepository()
        {
            Client = new DocumentClient(new Uri(Endpoint), AuthKey);
            Client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseId }).Wait();
            Client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseId),
                new DocumentCollection
                {
                    Id = CollectionId,
                    PartitionKey = new PartitionKeyDefinition
                    {
                        Paths = new Collection<string>(new List<string> { PartitionKeyPath })
                    }
                }).Wait();
        }

        public async Task<T> GetItemAsync(string id, string partitionKey)
        {
            try
            {
                var document =
                    await Client.ReadDocumentAsync<T>(
                        UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id),
                        new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
                return document.Document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            var query = Client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                    new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(predicate)
                .AsDocumentQuery();

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<Document> CreateItemAsync(T item)
        {
            return await Client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                item);
        }

        public async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await Client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id),
                item);
        }

        public async Task DeleteItemAsync(string id, string partitionKey)
        {
            await Client.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id),
                new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
        }

        public async Task Teardown()
        {
            await Client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
        }
    }
}
