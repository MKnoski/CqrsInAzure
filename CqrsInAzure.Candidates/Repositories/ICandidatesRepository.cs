using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CqrsInAzure.Candidates.Models;

namespace CqrsInAzure.Candidates.Repositories
{
    public interface ICandidatesRepository
    {
        Task<Candidate> GetItemAsync(string id, string partitionKey);

        Task<IEnumerable<Candidate>> GetItemsAsync(Expression<Func<Candidate, bool>> predicate);

        Task<string> CreateItemAsync(Candidate item);

        Task UpdateItemAsync(string id, string partitionKey, Candidate item);

        Task DeleteItemAsync(string id, string partitionKey);
    }
}