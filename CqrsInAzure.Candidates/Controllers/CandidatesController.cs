using System;
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Internal;

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
        private readonly IRequestRepository requestRepository;

        public CandidatesController(ICandidatesRepository repository, ICvStorage cvStorage, IPhotosStorage photosStorage, ICandidateEventPublisher candidateCreatedEventPublisher, IRequestRepository requestRepository)
        {
            this.repository = repository;
            this.cvStorage = cvStorage;
            this.photosStorage = photosStorage;
            this.candidateCreatedEventPublisher = candidateCreatedEventPublisher;
            this.requestRepository = requestRepository;
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
        public async Task<string> PostAsync([FromBody] Candidate candidate)
        {
            var request = new Request("Candidate creation");
            await requestRepository.CreateItemAsync(request);

            new Task(async () => { await CreateCandidateAsync(request, candidate); }).RunSynchronously();

            return request.Id;
        }

        [HttpPut("{id}/{partitionKey}")]
        public async Task PutAsync(string id, string partitionKey, [FromBody] Candidate updateCandidate)
        {
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

        private async Task CreateCandidateAsync(Request request, Candidate candidate)
        {
            await Task.Delay(60000);

            await this.repository.CreateItemAsync(candidate);

            request.RequestStatus = RequestStatus.Succeeded;
            request.ItemId = candidate.Id;

            await this.requestRepository.UpdateItemAsync(request.Id, request.Id, request);

            await this.candidateCreatedEventPublisher.PublishAsync("cqrsInAzure/candidate/created", candidate);
        }
    }
}