using System;
using Microsoft.Azure.Search;

namespace CqrsInAzure.Search.Models
{
    public class Education
    {
        [IsSearchable, IsFilterable]
        public string Name { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}
