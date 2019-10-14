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

        public async Task UpdateCandidateAsync(Candidate candidate, string newCategoryName)
        {
            await this.DeleteItemAsync(candidate.Id, candidate.CategoryName);

            candidate.CategoryName = newCategoryName;

            await this.CreateItemAsync(candidate);
        }
    }
}