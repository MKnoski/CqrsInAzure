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

        IList<SearchResult<Candidate>> SearchDocuments(
            string searchText,
            string filter,
            int page,
            int pageSize,
            IList<string> orderBy,
            IList<string> searchParameters);
    }
}