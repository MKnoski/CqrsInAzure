namespace CqrsInAzure.Search.Models
{
    public class CandidateUpdatedEventData
    {
        public Candidate OldCandidate { get; set; }

        public Candidate NewCandidate { get; set; }
    }
}