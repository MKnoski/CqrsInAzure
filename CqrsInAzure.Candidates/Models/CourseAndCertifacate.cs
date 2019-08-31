using Newtonsoft.Json;
using System;

namespace CqrsInAzure.Candidates.Models
{
    public class CourseAndCertifacate
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "other")]
        public string Other { get; set; } = "";
    }
}
