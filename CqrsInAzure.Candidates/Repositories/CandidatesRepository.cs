using CqrsInAzure.Candidates.Models;
using System.Threading.Tasks;

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