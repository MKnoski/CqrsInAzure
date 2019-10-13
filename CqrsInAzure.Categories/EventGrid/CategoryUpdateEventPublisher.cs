using CqrsInAzure.Categories.EventGrid.Models;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Threading.Tasks;

namespace CqrsInAzure.Categories.EventGrid
{
    public class CategoryUpdateEventPublisher : ICategoryUpdateEventPublisher
    {
        private const string TopicAuthKey = "oNls2kbWv4Ql4sxb1uTK51OCG2of9PM4gd83aWL/Ixw=";
        private const string TopicEndpoint = "https://cqrsinnazure-categoryupdate.westeurope-1.eventgrid.azure.net/api/events";
        private readonly EventGridClient eventGridClient;

        public CategoryUpdateEventPublisher()
        {
            var credentials = new TopicCredentials(TopicAuthKey);
            this.eventGridClient = new EventGridClient(credentials);
        }

        public async Task PublishCategoryUpdatedEventAsync(CategoryUpdatedEventData eventData)
        {
            var topicHostName = new Uri(TopicEndpoint).Host;
            var eventSubject = "cqrsInAzure/categories/categoryUpdated";

            await this.eventGridClient.PublishEventsAsync(
                topicHostName,
                new[] {
                    CreateEventGridEvent(eventSubject, eventData)
                });
        }

        private EventGridEvent CreateEventGridEvent(string eventSubject, object eventData)
        {
            return new EventGridEvent
            {
                Subject = eventSubject,
                Id = Guid.NewGuid().ToString(),
                EventType = eventData.GetType().FullName,
                Data = eventData,
                EventTime = DateTime.UtcNow,
                DataVersion = "1.0",
            };
        }
    }
}
