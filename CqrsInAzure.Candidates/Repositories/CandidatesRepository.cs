using CqrsInAzure.Candidates.Models;

namespace CqrsInAzure.Candidates.Repositories
{
    public class CandidatesRepository : DocumentDbRepository<Candidate>
    {
        public CandidatesRepository() 
            : base("Candidates", "/categoryId")
        {
        }
    }
}
