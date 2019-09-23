using System;
using Microsoft.Azure.Search;

namespace CqrsInAzure.Search.Models
{
    public class Experience
    {
        [IsSearchable]
        public string Name { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}
