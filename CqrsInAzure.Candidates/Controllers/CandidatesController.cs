using System;
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
        private readonly CandidatesRepository repository;
        private readonly CvStorage cvStorage;
        private readonly PhotosStorage photosStorage;

        public CandidatesController()
        {
            this.repository = new CandidatesRepository();
            this.cvStorage = new CvStorage();
            this.photosStorage = new PhotosStorage();
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
            var document = await this.repository.CreateItemAsync(candidate);

            return $"{document.Id}/{candidate.CategoryName}";
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
            await this.repository.DeleteItemAsync(id, partitionKey);
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
