using CqrsInAzure.Categories.EventGrid.Models;
using System.Threading.Tasks;

namespace CqrsInAzure.Categories.EventGrid
{
    public interface IEventPublisher
    {
        Task PublishCategoryUpdatedEventAsync(CategoryUpdatedEventData eventData);
    }
}
