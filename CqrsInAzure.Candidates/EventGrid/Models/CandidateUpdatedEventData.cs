using CqrsInAzure.Candidates.Models;

namespace CqrsInAzure.Candidates.EventGrid.Models
{
    public class CandidateUpdatedEventData
    {
        public Candidate OldCandidate { get; set; }

        public Candidate NewCandidate { get; set; }
    }
}