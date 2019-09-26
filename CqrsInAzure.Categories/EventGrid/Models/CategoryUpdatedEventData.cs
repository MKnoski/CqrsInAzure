namespace CqrsInAzure.Categories.EventGrid.Models
{
    public class CategoryUpdatedEventData
    {
        public string OldCategoryName { get; set; }

        public string NewCategoryName { get; set; }
    }
}
