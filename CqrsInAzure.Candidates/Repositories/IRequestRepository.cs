using System.Threading.Tasks;
using CqrsInAzure.Candidates.Models;

namespace CqrsInAzure.Candidates.Repositories
{
    public interface IRequestRepository
    {
        Task<Request> GetItemAsync(string id, string partitionKey);

        Task CreateItemAsync(Request item);

        Task UpdateItemAsync(string id, string partitionKey, Request item);
    }
}