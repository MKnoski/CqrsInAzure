using CqrsInAzure.Categories.EventGrid.Models;
using System.Threading.Tasks;

namespace CqrsInAzure.Categories.Publishers.EventGrid
{
    public interface ICategoryEventPublisher
    {
        Task PublishAsync(string eventSubject, object eventData);
    }
}