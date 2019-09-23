using System;
using Microsoft.Azure.Search;

namespace CqrsInAzure.Search.Models
{
    public class CourseAndCertificate
    {
        [IsSearchable, IsFilterable]
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Other { get; set; } = "";
    }
}
