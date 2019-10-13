using System.Collections.Generic;
using System.Threading.Tasks;
using CqrsInAzure.Search.Models;
using Microsoft.Azure.Search.Models;

namespace CqrsInAzure.Search.Clients
{
    public interface ICandidatesSearchClient
    {
        Task CreateIndexIfNotExists();
        Task CreateIndexerIfNotExists();
        void InsertCandidates(List<Candidate> candidates);
        void InsertOrUpdateCandidates(List<Candidate> candidates);
        void UpdateCandidates(List<Candidate> candidates);
        void DeleteCandidates(List<Candidate> candidates);

        Task<IEnumerable<SearchResult<Candidate>>> SearchDocumentsAsync(
            string searchText = null,
            string filter = null,
            int? page = null,
            int? pageSize = null,
            IList<string> orderBy = null,
            IList<string> searchParameters = null,
            IList<string> searchFields = null);
    }
}