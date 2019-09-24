using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CqrsInAzure.Search.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace CqrsInAzure.Search.Clients
{
    public class CandidatesSearchClient : ICandidatesSearchClient
    {
        private const string SearchServiceName = "cqrs-in-azure";
        private const string IndexName = "candidates-index";
        private const string IndexerName = "candidates-indexer";
        private const string AdminApiKey = "4F1E9367685862FF7C26F213BED28D2C";

        private const string CosmosDBConnectionString = "https://cqrs-in-azure.documents.azure.com";
        private const string CosmosDBDatabaseName = "cqrs-in-azure";
        private const string CollectionName = "Candidates";
        protected readonly string AuthKey = "6W5mEPbFOpv1CSvBHOwcgPJdxtip0CEwqPvjZ79ydffwFYOkHcHZrKbzLdFJRCLXThJUI8otQyJKk1HRWSozHw==";

        private readonly SearchServiceClient searchClient;
        private readonly ISearchIndexClient indexClient;

        public CandidatesSearchClient()
        {
            this.searchClient = new SearchServiceClient(SearchServiceName, new SearchCredentials(AdminApiKey));
            this.indexClient = this.searchClient.Indexes.GetClient(IndexName);

            CreateIndexIfNotExists().Wait();
            CreateIndexerIfNotExists().Wait();
        }

        public async Task CreateIndexIfNotExists()
        {
            if (await this.searchClient.Indexes.ExistsAsync(IndexName))
            {
                return;
            }

            try
            {
                var index = new Index
                {
                    Name = IndexName,
                    Fields = FieldBuilder.BuildForType<Candidate>()
                };
                await this.searchClient.Indexes.CreateAsync(index);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task CreateIndexerIfNotExists()
        {
            if (await this.searchClient.Indexers.ExistsAsync(IndexerName))
            {
                return;
            }

            try
            {
                var cosmosDbConnectionString = $"AccountEndpoint={CosmosDBConnectionString};AccountKey={AuthKey};Database={CosmosDBDatabaseName}";

                DataSource cosmosDbDataSource = DataSource.CosmosDb(
                    name: CosmosDBDatabaseName,
                    cosmosDbConnectionString: cosmosDbConnectionString,
                    collectionName: CollectionName);

                cosmosDbDataSource.DataDeletionDetectionPolicy = new SoftDeleteColumnDeletionDetectionPolicy("isDeleted", true);
                
                await this.searchClient.DataSources.CreateOrUpdateAsync(cosmosDbDataSource);

                Indexer cosmosDbIndexer = new Indexer(
                    name: IndexerName,
                    dataSourceName: cosmosDbDataSource.Name,
                    targetIndexName: IndexName,
                    schedule: new IndexingSchedule(TimeSpan.FromDays(1)));

                await this.searchClient.Indexers.CreateOrUpdateAsync(cosmosDbIndexer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void InsertCandidates(List<Candidate> candidates)
        {
            // An upload action is similar to an "upsert" where the document will be inserted if it is new and updated/replaced if it exists
            var batch = IndexBatch.Upload(candidates);
            ExecuteBatch(batch);
        }

        public void InsertOrUpdateCandidates(List<Candidate> candidates)
        {
            // This action behaves like merge if a document with the given key already exists in the index.
            // If the document does not exist, it behaves like upload with a new document.
            var batch = IndexBatch.MergeOrUpload(candidates);
            ExecuteBatch(batch);
        }

        public void UpdateCandidates(List<Candidate> candidates)
        {
            // Merge updates an existing document with the specified fields.
            // If the document doesn't exist, the merge will fail.
            // Any field you specify in a merge will replace the existing field in the document
            var batch = IndexBatch.Merge(candidates);
            ExecuteBatch(batch);
        }

        public void DeleteCandidates(List<Candidate> candidates)
        {
            // Delete removes the specified document from the index
            var batch = IndexBatch.Delete(candidates);
            ExecuteBatch(batch);
        }

        public IList<SearchResult<Candidate>> SearchDocuments(
            string searchText,
            string filter,
            int page,
            int pageSize,
            IList<string> orderBy,
            IList<string> searchParameters)
        {
            DocumentSearchResult<Candidate> results;
            try
            {
                var parameters =
                    new SearchParameters
                    {
                        Select = searchParameters,
                        Top = pageSize,
                        Skip = (page - 1) * pageSize,
                        OrderBy = orderBy,
                        //Facets = new[] { "CategoryName" },
                        Filter = filter
                    };

                results = this.indexClient.Documents.Search<Candidate>(searchText, parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            WriteDocuments(results);
            return results.Results;
        }

        private void ExecuteBatch(IndexBatch<Candidate> batch)
        {
            try
            {
                this.indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                Console.WriteLine("Failed to index some of the documents: {0}",
                    string.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
        }

        private void WriteDocuments(DocumentSearchResult<Candidate> searchResults)
        {
            foreach (var result in searchResults.Results)
            {
                Console.WriteLine(result.Document);
            }

            Console.WriteLine();
        }

        private void DeleteIndexIfExists()
        {
            if (this.searchClient.Indexes.Exists(IndexName))
            {
                this.searchClient.Indexers.Delete(IndexName);
            }
        }
    }
}