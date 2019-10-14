using System;
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
        private const string CandidateCreatedEventSubject = "cqrsInAzure/candidates/created";

        private ICategoriesStorage storage;

        public SyncController(ICategoriesStorage storage)
        {
            this.storage = storage;
        }

        [SubscriptionValidation]
        [HttpPost("increseCategoryUsage")]
        public async Task<IActionResult> IncreaseCategoryUsageAsync([FromBody] object eventData)
        {
            var eventGridEvent = JsonConvert.DeserializeObject<EventGridEvent[]>(eventData.ToString()).FirstOrDefault();

            if (eventGridEvent == null)
            {
                return BadRequest();
            }

            var data = eventGridEvent.Data as JObject;

            if (IsCandidateCreatedEvent(eventGridEvent))
            {
                var candidateCreatedEventData = data.ToObject<CandidateCreatedEventData>() as CandidateCreatedEventData;
                var categoryName = candidateCreatedEventData.CategoryName;

                var category = await this.storage.GetAsync(categoryName);

                category.AssignedCandidates++;

                await this.storage.UpdateAsync(categoryName, category);
            }

            return Ok();
        }

        private static bool IsCandidateCreatedEvent(EventGridEvent eventGridEvent)
        {
            return string.Equals(eventGridEvent.Subject, CandidateCreatedEventSubject, StringComparison.OrdinalIgnoreCase);
        }
    }
}