using System;
using System.Linq;
using System.Threading.Tasks;
using CqrsInAzure.Candidates.Attributes;
using CqrsInAzure.Candidates.Models;
using CqrsInAzure.Candidates.Repositories;
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
        private const string CategoryUpdatedEventSubject = "cqrsInAzure/categories/categoryUpdated";

        private readonly ICandidatesRepository repository;

        public SyncController(ICandidatesRepository repository)
        {
            this.repository = repository;
        }

        [SubscriptionValidation]
        [HttpPost("updateCategory")]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] object eventData)
        {
            var eventGridEvent = JsonConvert.DeserializeObject<EventGridEvent[]>(eventData.ToString()).FirstOrDefault();

            if (eventGridEvent == null)
            {
                return BadRequest();
            }
            
            var data = eventGridEvent.Data as JObject;

            if (IsCategoryUpdatedEvent(eventGridEvent))
            {
                var categoryUpdatedEventData = data.ToObject<CategoryUpdatedEventData>() as CategoryUpdatedEventData;
                var candidatesWithOldCategory = (await this.repository.GetItemsAsync(m => m.CategoryName == categoryUpdatedEventData.OldCategoryName, false)).ToList();
                candidatesWithOldCategory.ForEach(async c => await UpdateCandidatesAsync(c, categoryUpdatedEventData.NewCategoryName));
            }

            return Ok();
        }

        private async Task UpdateCandidatesAsync(Candidate candidate, string newCategoryName)
        {
            await this.repository.DeleteItemAsync(candidate.Id, candidate.CategoryName);

            candidate.CategoryName = newCategoryName;

            await this.repository.CreateItemAsync(candidate);
        }

        private static bool IsCategoryUpdatedEvent(EventGridEvent eventGridEvent)
        {
            return string.Equals(eventGridEvent.Subject, CategoryUpdatedEventSubject, StringComparison.OrdinalIgnoreCase);
        }
    }
}