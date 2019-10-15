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
        private const string CategoryUpdatedEventSubject = "cqrsInAzure/category/updated";

        private const string CandidateCreatedEventSubject = "cqrsInAzure/candidate/created";
        private const string CandidateDeletedEventSubject = "cqrsInAzure/candidate/deleted";
        private const string CandidatesCategoryUpdatedEventSubject = "cqrsInAzure/candidate/category/updated";

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

            if (string.Equals(eventGridEvent.Subject, CategoryUpdatedEventSubject, StringComparison.OrdinalIgnoreCase))
            {
                var categoryUpdatedEventData = data.ToObject<CategoryUpdatedEventData>() as CategoryUpdatedEventData;

                var searchResults = await this.candidatesSearchClient.SearchDocumentsAsync(searchText: categoryUpdatedEventData.OldCategoryName, searchFields: new List<string> { nameof(Candidate.CategoryName) });

                var candidates = searchResults.Select(s => s.Document).ToList();

                if (candidates.Count > 0)
                {
                    candidates.ForEach(c => c.CategoryName = categoryUpdatedEventData.NewCategoryName);
                    await this.candidatesSearchClient.InsertOrUpdateCandidatesAsync(candidates);
                }
            }

            return Ok();
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
            var candidateCreatedEventData = data.ToObject<Candidate>() as Candidate;

            await this.candidatesSearchClient.InsertCandidatesAsync(candidateCreatedEventData.ToList());
        }

        private async Task HandleCandidatesCategoryUpdatedEventAsync(JObject data)
        {
            var candidateUpdatedEventData = data.ToObject<CandidateUpdatedEventData>() as CandidateUpdatedEventData;

            await this.candidatesSearchClient.UpdateCandidatesAsync(candidateUpdatedEventData.NewCandidate.ToList());
        }

        private async Task HandleCandidateDeletedEventAsync(JObject data)
        {
            var candidateDeletedEventData = data.ToObject<Candidate>() as Candidate;

            await this.candidatesSearchClient.DeleteCandidatesAsync(candidateDeletedEventData.ToList());
        }
    }
}