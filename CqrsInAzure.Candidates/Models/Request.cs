using Newtonsoft.Json;

namespace CqrsInAzure.Candidates.Models
{
    public class Request : Deletable, IIdentifiable
    {
        public Request(string description = "")
        {
            Description = description;
            RequestStatus = RequestStatus.Initialized;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "itemId")]
        public string ItemId { get; set; }

        [JsonProperty(PropertyName = "requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}