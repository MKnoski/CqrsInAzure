using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace CqrsInAzure.Candidates.Attributes
{
    public class SubscriptionValidationAttribute : ActionFilterAttribute
    {
        private const string SubscriptionValidationEvent = "Microsoft.EventGrid.SubscriptionValidationEvent";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var eventData = filterContext.ActionArguments["eventData"];

            var eventGridEvent = JsonConvert.DeserializeObject<EventGridEvent[]>(eventData.ToString()).FirstOrDefault();

            if (eventGridEvent == null)
            {
                return;
            }
            
            if (IsSubscriptionValidationEvent(eventGridEvent))
            {
                var data = (eventGridEvent.Data as JObject).ToObject<SubscriptionValidationEventData>();

                var responseData = new SubscriptionValidationResponse
                {
                    ValidationResponse = data.ValidationCode
                };

                filterContext.Result = new OkObjectResult(responseData);
            }
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

            return new OkObjectResult(responseData);
        }
    }
}
