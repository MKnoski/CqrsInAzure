using CqrsInAzure.Categories.EventGrid.Models;
using System.Threading.Tasks;

namespace CqrsInAzure.Categories.EventGrid
{
    public interface ICategoryUpdateEventPublisher
    {
        Task PublishCategoryUpdatedEventAsync(CategoryUpdatedEventData eventData);
    }
}
