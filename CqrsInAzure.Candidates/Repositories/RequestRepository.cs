using CqrsInAzure.Candidates.Models;

namespace CqrsInAzure.Candidates.Repositories
{
    public class RequestRepository : DocumentDbRepository<Request>, IRequestRepository
    {
        public RequestRepository()
            : base("Requests", "/id")
        {
        }
    }
}