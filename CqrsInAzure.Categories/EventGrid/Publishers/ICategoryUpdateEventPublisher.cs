using CqrsInAzure.Categories.EventGrid.Models;
using System.Threading.Tasks;

namespace CqrsInAzure.Categories.Publishers.EventGrid
{
    public interface ICategoryUpdateEventPublisher
    {
        Task PublishAsync(CategoryUpdatedEventData eventData);
    }
}
