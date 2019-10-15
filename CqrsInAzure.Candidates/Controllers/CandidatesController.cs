using CqrsInAzure.Candidates.EventGrid.Models;
using CqrsInAzure.Candidates.EventGrid.Publishers;
using CqrsInAzure.Candidates.Helpers;
using CqrsInAzure.Candidates.Models;
using CqrsInAzure.Candidates.Repositories;
using CqrsInAzure.Candidates.Storage;
using CqrsInAzure.Candidates.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CqrsInAzure.Candidates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidatesRepository repository;
        private readonly ICvStorage cvStorage;
        private readonly IPhotosStorage photosStorage;
        private readonly ICandidateEventPublisher candidateCreatedEventPublisher;

        public CandidatesController(ICandidatesRepository repository, ICvStorage cvStorage, IPhotosStorage photosStorage, ICandidateEventPublisher candidateCreatedEventPublisher)
        {
            this.repository = repository;
            this.cvStorage = cvStorage;
            this.photosStorage = photosStorage;
            this.candidateCreatedEventPublisher = candidateCreatedEventPublisher;
        }

        [HttpGet]
        public async Task<IEnumerable<CandidateViewModel>> GetAsync()
        {
            var candidates = await this.repository.GetItemsAsync(candidate => true);

            return candidates.Select(Map);
        }

        [HttpGet("{id}/{partitionKey}", Name = "Get")]
        public async Task<CandidateViewModel> GetAsync(string id, string partitionKey)
        {
            var candidate = await this.repository.GetItemAsync(id, partitionKey);

            if (candidate == null)
            {
                return null;
            }

            return Map(candidate);
        }

        [HttpPost]
        public async Task PostAsync([FromBody] Candidate candidate)
        {
            // TODO Validate if category exists

            await this.repository.CreateItemAsync(candidate);

            await this.candidateCreatedEventPublisher.PublishAsync("cqrsInAzure/candidate/created", candidate);
        }

        [HttpPut("{id}/{partitionKey}")]
        public async Task PutAsync(string id, string partitionKey, [FromBody] Candidate updateCandidate)
        {
            // TODO Validate if category exists

            var originalCandidate = await this.repository.GetItemAsync(id, partitionKey);
            var newCandidate = updateCandidate.Merge(originalCandidate);

            string newCategoryName = default;
            if (!string.IsNullOrEmpty(newCandidate.CategoryName) && newCandidate.CategoryName != originalCandidate.CategoryName)
            {
                newCategoryName = newCandidate.CategoryName;
            }

            if (newCategoryName != null)
            {
                await this.repository.ReuploadItemAsync(originalCandidate.Id, originalCandidate.CategoryName, newCandidate);

                var eventData = new CandidateUpdatedEventData
                {
                    OldCandidate = originalCandidate,
                    NewCandidate = newCandidate
                };
                await this.candidateCreatedEventPublisher.PublishAsync("cqrsInAzure/candidate/updated", eventData);
            }
            else
            {
                await this.repository.UpdateItemAsync(originalCandidate.Id, originalCandidate.CategoryName, newCandidate);
            }
        }

        [HttpDelete("{id}/{partitionKey}")]
        public async Task DeleteAsync(string id, string partitionKey)
        {
            var candidate = await this.repository.GetItemAsync(id, partitionKey);

            if (candidate == null)
            {
                return;
            }

            await this.cvStorage.DeleteAsync(candidate.CvId);
            await this.photosStorage.DeleteAsync(candidate.PhotoId);

            await this.repository.DeleteSoftItemAsync(id, partitionKey);

            await this.candidateCreatedEventPublisher.PublishAsync("cqrsInAzure/candidate/deleted", candidate);
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