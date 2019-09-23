using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;

namespace CqrsInAzure.Search.Models
{
    public class Candidate
    {
        public Candidate(string id)
        {
            Id = id;
        }

        [Key]
        public string Id { get; set; }

        [IsSearchable, IsSortable]
        public string FirstName { get; set; }

        [IsSearchable, IsSortable]
        public string LastName { get; set; }

        [IsSearchable]
        public string Address { get; set; }

        public string[] Skills { get; set; }

        [IsSearchable, IsFilterable, IsSortable]
        public string CategoryName { get; set; }

        public string CvId { get; set; }

        public CourseAndCertificate[] CoursesAndCertificates { get; set; }

        public Experience[] Experience { get; set; }

        public string PhotoId { get; set; }

        public Education[] Education { get; set; }
    }

    public static class CandidateExtensions
    {
        public static List<Candidate> ToList(this Candidate candidate)
        {
            return new List<Candidate> { candidate };
        }

        public static Candidate CreateCandidate(this string id)
        {
            return new Candidate(id);
        }
    }
}