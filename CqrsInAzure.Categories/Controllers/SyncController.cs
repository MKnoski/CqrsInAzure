using System.Linq;
using System.Threading.Tasks;
using CqrsInAzure.Categories.Attributes;
using CqrsInAzure.Categories.EventGrid.Models;
using CqrsInAzure.Categories.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CqrsInAzure.Candidates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private const string CandidateCreatedEventSubject = "cqrsInAzure/candidate/created";
        private const string CandidateDeletedEventSubject = "cqrsInAzure/candidate/deleted";
        private const string CandidatesCategoryUpdatedEventSubject = "cqrsInAzure/candidate/updated";

        private ICategoriesStorage storage;

        public SyncController(ICategoriesStorage storage)
        {
            this.storage = storage;
        }

        [SubscriptionValidation]
        [HttpPost("handleCandidateModification")]
        public async Task<IActionResult> HandleCandidateModification([FromBody] object eventData)
        {
            var eventGridEvent = JsonConvert.DeserializeObject<EventGridEvent[]>(eventData.ToString()).FirstOrDefault();

            if (eventGridEvent == null)
            {
                return BadRequest();
            }

            var data = eventGridEvent.Data as JObject;

            switch (eventGridEvent.Subject)
            {
                case CandidateCreatedEventSubject:
                    await HandleCandidateCreatedEventAsync(data);
                    break;

                case CandidateDeletedEventSubject:
                    await HandleCandidateDeletedEventAsync(data);
                    break;

                case CandidatesCategoryUpdatedEventSubject:
                    await HandleCandidatesCategoryUpdatedEventAsync(data);
                    break;
            }

            return Ok();
        }

        private async Task HandleCandidateCreatedEventAsync(JObject data)
        {
            var candidateCreatedEventData = data.ToObject<CandidateEventData>() as CandidateEventData;
            await IncreaseCategoryUsage(candidateCreatedEventData.CategoryName);
        }

        private async Task HandleCandidatesCategoryUpdatedEventAsync(JObject data)
        {
            var candidateUpdatedEventData = data.ToObject<CandidateUpdatedEventData>() as CandidateUpdatedEventData;
            await DecreaseCategoryUsage(candidateUpdatedEventData.OldCandidate.CategoryName);
            await IncreaseCategoryUsage(candidateUpdatedEventData.NewCandidate.CategoryName);
        }

        private async Task HandleCandidateDeletedEventAsync(JObject data)
        {
            var candidateDeletedEventData = data.ToObject<CandidateEventData>() as CandidateEventData;
            await DecreaseCategoryUsage(candidateDeletedEventData.CategoryName);
        }

        private async Task IncreaseCategoryUsage(string categoryName)
        {
            var category = await this.storage.GetAsync(categoryName);

            if (category == null)
                return;

            category.AssignedCandidates++;

            await this.storage.UpdateAsync(categoryName, category);
        }

        private async Task DecreaseCategoryUsage(string categoryName)
        {
            var category = await this.storage.GetAsync(categoryName);

            if (category == null)
                return;

            category.AssignedCandidates--;

            await this.storage.UpdateAsync(categoryName, category);
        }
    }
}