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

namespace CqrsInAzure.Candidates.Repositories
{
    public abstract class DocumentDbRepository<T>
        where T : class
    {
        protected readonly DocumentClient Client;

        protected readonly string CollectionId;
        protected readonly string PartitionKeyPath;

        // move to settings
        protected readonly string DatabaseId = "cqrs-in-azure";
        protected readonly string Endpoint = "https://cqrs-in-azure.documents.azure.com:443/";
        protected readonly string AuthKey = "6W5mEPbFOpv1CSvBHOwcgPJdxtip0CEwqPvjZ79ydffwFYOkHcHZrKbzLdFJRCLXThJUI8otQyJKk1HRWSozHw==";

        protected DocumentDbRepository(string collectionId, string partitionKeyPath)
        {
            CollectionId = collectionId;
            PartitionKeyPath = partitionKeyPath;

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

        public async Task<string> CreateItemAsync(T item)
        {
            var document = await Client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                item);

            return document.Resource.Id;
        }

        public async Task UpdateItemAsync(string id, string partitionKey, T item)
        {
             await Client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id),
                item,
                new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
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
