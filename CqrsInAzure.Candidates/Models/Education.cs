using Newtonsoft.Json;
using System;

namespace CqrsInAzure.Candidates.Models
{
    [Serializable]
    public class Education
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "from")]
        public DateTime? From { get; set; }

        [JsonProperty(PropertyName = "to")]
        public DateTime? To { get; set; }
    }
}