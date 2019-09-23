
using CqrsInAzure.Candidates.Models;

namespace CqrsInAzure.Candidates.ViewModels
{
    public class CandidateViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string[] Skills { get; set; }

        public string CategoryName { get; set; }

        public string CvLink { get; set; }

        public CourseAndCertificate[] CoursesAndCertificates { get; set; }

        public Experience[] Experience { get; set; }

        public string PhotoLink { get; set; }

        public Education[] Education { get; set; }
    }
}
