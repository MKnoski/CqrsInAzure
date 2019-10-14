namespace CqrsInAzure.Categories.EventGrid.Models
{
    public class CandidateCreatedEventData
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string CategoryName { get; set; }
    }
}