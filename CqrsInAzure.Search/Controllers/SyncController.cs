using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CqrsInAzure.Search.Attributes;
using CqrsInAzure.Search.Clients;
using CqrsInAzure.Search.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CqrsInAzure.Search.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private const string CategoryUpdatedEventSubject = "cqrsInAzure/categories/categoryUpdated";

        private readonly ICandidatesSearchClient candidatesSearchClient;

        public SyncController(ICandidatesSearchClient candidatesSearchClient)
        {
            this.candidatesSearchClient = candidatesSearchClient;
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

                var searchResults = await this.candidatesSearchClient.SearchDocumentsAsync(searchText: categoryUpdatedEventData.OldCategoryName, searchFields: new List<string> { nameof(Candidate.CategoryName) });

                var candidates = searchResults.Select(s => s.Document).ToList();

                if (candidates.Count > 0)
                {
                    candidates.ForEach(c => c.CategoryName = categoryUpdatedEventData.NewCategoryName);
                    this.candidatesSearchClient.InsertOrUpdateCandidates(candidates);
                }
            }

            return Ok();
        }

        private static bool IsCategoryUpdatedEvent(EventGridEvent eventGridEvent)
        {
            return string.Equals(eventGridEvent.Subject, CategoryUpdatedEventSubject, StringComparison.OrdinalIgnoreCase);
        }
    }
}