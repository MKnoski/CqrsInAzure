using System.Collections.Generic;
using System.Threading.Tasks;
using CqrsInAzure.Candidates.Models;
using CqrsInAzure.Candidates.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CqrsInAzure.Candidates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly CandidatesRepository repository;

        public CandidatesController()
        {
            this.repository = new CandidatesRepository();
        }

        [HttpGet]
        public async Task<IEnumerable<Candidate>> Get()
        {
            var candidates = await this.repository.GetItemsAsync(candidate => true);

            return candidates;
        }

        [HttpGet("{id}/{partitionKey}", Name = "Get")]
        public async Task<Candidate> Get(string id, string partitionKey)
        {
            var candidate = await this.repository.GetItemAsync(id, partitionKey);

            return candidate;
        }

        [HttpPost]
        public async Task Post([FromBody] Candidate candidate)
        {
            await this.repository.CreateItemAsync(candidate);
        }

        [HttpPut("{id}/{partitionKey}")]
        public async Task Put(string id, string partitionKey, [FromBody] Candidate candidate)
        {
            candidate.Id = id;
            candidate.CategoryId = partitionKey;

            await this.repository.UpdateItemAsync(id, partitionKey, candidate);
        }

        [HttpDelete("{id}/{partitionKey}")]
        public async Task Delete(string id, string partitionKey)
        {
            await this.repository.DeleteItemAsync(id, partitionKey);
        }
    }
}
