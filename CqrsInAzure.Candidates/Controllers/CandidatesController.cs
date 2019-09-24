using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CqrsInAzure.Candidates.Models;
using CqrsInAzure.Candidates.Repositories;
using CqrsInAzure.Candidates.Storage;
using CqrsInAzure.Candidates.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CqrsInAzure.Candidates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidatesRepository repository;
        private readonly ICvStorage cvStorage;
        private readonly IPhotosStorage photosStorage;

        public CandidatesController(ICandidatesRepository repository, ICvStorage cvStorage, IPhotosStorage photosStorage)
        {
            this.repository = repository;
            this.cvStorage = cvStorage;
            this.photosStorage = photosStorage;
        }

        [HttpGet]
        public async Task<IEnumerable<CandidateViewModel>> Get()
        {
            var candidates = await this.repository.GetItemsAsync(candidate => true);

            return candidates.Select(Map);
        }

        [HttpGet("{id}/{partitionKey}", Name = "Get")]
        public async Task<CandidateViewModel> Get(string id, string partitionKey)
        {
            var candidate = await this.repository.GetItemAsync(id, partitionKey);

            if (candidate == null)
            {
                return null;
            }

            return Map(candidate);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Candidate candidate)
        {
            var id = await this.repository.CreateItemAsync(candidate);

            return $"{id}/{candidate.CategoryName}";
        }

        [HttpPut("{id}/{partitionKey}")]
        public async Task Put(string id, string partitionKey, [FromBody] Candidate candidate)
        {
            candidate.Id = id;
            candidate.CategoryName = partitionKey;

            await this.repository.UpdateItemAsync(id, partitionKey, candidate);
        }

        [HttpDelete("{id}/{partitionKey}")]
        public async Task Delete(string id, string partitionKey)
        {
            var candidate = await this.repository.GetItemAsync(id, partitionKey);

            if (candidate == null)
            {
                return;
            }

            await this.cvStorage.DeleteAsync(candidate.CvId);
            await this.photosStorage.DeleteAsync(candidate.PhotoId);

            await this.repository.DeleteSoftItemAsync(id, partitionKey);
        }

        private CandidateViewModel Map(Candidate candidate)
        {
            var cvLink = this.cvStorage.GetLink(candidate.CvId);
            var photoLink = this.photosStorage.GetLink(candidate.PhotoId);

            return new CandidateViewModel
            {
                Id = candidate.Id,
                CategoryName = candidate.CategoryName,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                Address = candidate.Address,
                CoursesAndCertificates = candidate.CoursesAndCertificates,
                Skills = candidate.Skills,
                Education = candidate.Education,
                Experience = candidate.Experience,
                CvLink = cvLink,
                PhotoLink = photoLink
            };
        }
    }
}
