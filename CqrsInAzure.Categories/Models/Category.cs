using Newtonsoft.Json;
using System;

namespace CqrsInAzure.Categories.Models
{
    [Serializable]
    public class Category
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "sortOrder")]
        public SortOrder SortOrder { get; set; }
    }
}
