using Newtonsoft.Json;
using System;

namespace CqrsInAzure.Candidates.Models
{
    [Serializable]
    public class Deletable
    {
        [JsonProperty(PropertyName = "isDeleted")]
        public bool IsDeleted { get; set; }
    }
}