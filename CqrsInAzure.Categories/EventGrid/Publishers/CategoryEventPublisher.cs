using CqrsInAzure.Categories.EventGrid.Models;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Threading.Tasks;

namespace CqrsInAzure.Categories.Publishers.EventGrid
{
    public class CategoryEventPublisher : ICategoryEventPublisher
    {
        private const string TopicAuthKey = "/i60Wf3eU+xO+ewUANROjvMdFF2dXruFQe4RUl9OQEQ=";
        private const string TopicEndpoint = "https://cqrsinazure-category.westeurope-1.eventgrid.azure.net/api/events";
        private readonly EventGridClient eventGridClient;

        public CategoryEventPublisher()
        {
            var credentials = new TopicCredentials(TopicAuthKey);
            this.eventGridClient = new EventGridClient(credentials);
        }

        public async Task PublishAsync(string eventSubject, object eventData)
        {
            var topicHostName = new Uri(TopicEndpoint).Host;

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