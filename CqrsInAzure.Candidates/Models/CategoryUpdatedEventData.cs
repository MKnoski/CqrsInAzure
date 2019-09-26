namespace CqrsInAzure.Candidates.Models
{
    public class CategoryUpdatedEventData
    {
        public string OldCategoryName { get; set; }

        public string NewCategoryName { get; set; }
    }
}
