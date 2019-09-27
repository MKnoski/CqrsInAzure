using System;
using System.Linq;
using CqrsInAzure.Candidates.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CqrsInAzure.Candidates.Controllers
{
    public class SyncController : ControllerBase
    {
        private const string SubscriptionValidationEvent = "Microsoft.EventGrid.SubscriptionValidationEvent";
        private const string CategoryUpdatedEventSubject = "cqrsInAzure/categories/categoryUpdated";

        [HttpPost("updateCategory")]
        public IActionResult UpdateCategory([FromBody] object eventData)
        {
            var eventGridEvent = JsonConvert.DeserializeObject<EventGridEvent[]>(eventData.ToString()).FirstOrDefault();

            if (eventGridEvent == null)
            {
                return BadRequest();
            }
            
            var data = eventGridEvent.Data as JObject;

            // move to attribute
            if (IsSubscriptionValidationEvent(eventGridEvent))
            {
                return HandleSubscriptionValidation(data.ToObject<SubscriptionValidationEventData>());
            }

            if (IsCategoryUpdatedEvent(eventGridEvent))
            {
                var categoryUpdatedEventData = data.ToObject<CategoryUpdatedEventData>() as CategoryUpdatedEventData;
            }

            return Ok();
        }

        private static bool IsCategoryUpdatedEvent(EventGridEvent eventGridEvent)
        {
            return string.Equals(eventGridEvent.Subject, CategoryUpdatedEventSubject, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSubscriptionValidationEvent(EventGridEvent eventGridEvent)
        {
            return string.Equals(eventGridEvent.EventType, SubscriptionValidationEvent, StringComparison.OrdinalIgnoreCase);
        }

        private IActionResult HandleSubscriptionValidation(SubscriptionValidationEventData eventData)
        {
            var responseData = new SubscriptionValidationResponse
            {
                ValidationResponse = eventData.ValidationCode
            };

            return Ok(responseData);
        }
    }
}