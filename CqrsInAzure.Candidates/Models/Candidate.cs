using Newtonsoft.Json;

namespace CqrsInAzure.Candidates.Models
{
    public class Candidate
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "skills")]
        public string[] Skills { get; set; }

        [JsonProperty(PropertyName = "categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty(PropertyName = "cvId")]
        public string CvId { get; set; }

        [JsonProperty(PropertyName = "coursesAndCertificates")]
        public CourseAndCertificate[] CoursesAndCertificates { get; set; }

        [JsonProperty(PropertyName = "experiences")]
        public Experience[] Experience { get; set; }

        [JsonProperty(PropertyName = "photoId")]
        public string PhotoId { get; set; }

        [JsonProperty(PropertyName = "education")]
        public Education[] Education { get; set; }
    }
}
