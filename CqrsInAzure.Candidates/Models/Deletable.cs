using Newtonsoft.Json;

namespace CqrsInAzure.Candidates.Models
{
    public class Deletable
    {
        [JsonProperty(PropertyName = "isDeleted")]
        public bool IsDeleted { get; set; }
    }
}