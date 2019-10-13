using Newtonsoft.Json;
using System;

namespace CqrsInAzure.Categories.Models
{
    [Serializable]
    public class Category
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "sortOrder")]
        public SortOrder SortOrder { get; set; }

        [JsonProperty(PropertyName = "assignedCandidates")]
        public int AssignedCandidates { get; set; }
    }
}
