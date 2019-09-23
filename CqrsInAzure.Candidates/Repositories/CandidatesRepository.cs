using CqrsInAzure.Candidates.Models;

namespace CqrsInAzure.Candidates.Repositories
{
    public class CandidatesRepository : DocumentDbRepository<Candidate>, ICandidatesRepository
    {
        public CandidatesRepository() 
            : base("Candidates", "/categoryName")
        {
        }
    }
}
