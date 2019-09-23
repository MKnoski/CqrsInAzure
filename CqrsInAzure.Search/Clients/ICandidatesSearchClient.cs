using System.Collections.Generic;
using CqrsInAzure.Search.Models;
using Microsoft.Azure.Search.Models;

namespace CqrsInAzure.Search.Clients
{
    public interface ICandidatesSearchClient
    {
        void InsertCandidates(List<Candidate> candidates);

        void InsertOrUpdateCandidates(List<Candidate> candidates);

        void UpdateCandidates(List<Candidate> candidates);

        IList<SearchResult<Candidate>> SearchDocuments(
            string searchText,
            string filter = null,
            int page = 1,
            int pageSize = 10,
            IList<string> orderBy = null,
            IList<string> searchParameters = null);

        void DeleteCandidates(List<Candidate> candidates);
    }
}