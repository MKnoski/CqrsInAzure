using CqrsInAzure.Candidates.EventGrid.Models;
using CqrsInAzure.Candidates.Models;
using System.Threading.Tasks;

namespace CqrsInAzure.Candidates.EventGrid.Publishers
{
    public interface ICandidateEventPublisher
    {
        Task PublishAsync(string eventSubject, object eventData);
    }
}