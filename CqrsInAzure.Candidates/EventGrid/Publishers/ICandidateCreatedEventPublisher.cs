using CqrsInAzure.Candidates.EventGrid.Models;
using CqrsInAzure.Candidates.Models;
using System.Threading.Tasks;

namespace CqrsInAzure.Candidates.EventGrid.Publishers
{
    public interface ICandidateCreatedEventPublisher
    {
        Task PublishAsync(Candidate eventData);
    }
}