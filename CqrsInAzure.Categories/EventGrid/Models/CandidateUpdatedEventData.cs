namespace CqrsInAzure.Categories.EventGrid.Models
{
    public class CandidateUpdatedEventData
    {
        public CandidateEventData OldCandidate { get; set; }

        public CandidateEventData NewCandidate { get; set; }
    }
}