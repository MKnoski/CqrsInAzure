using System.Collections.Generic;
using System.Linq;
using CqrsInAzure.Search.Clients;
using CqrsInAzure.Search.Models;
using Microsoft.AspNetCore.Mvc;

namespace CqrsInAzure.Search.Controllers
{
    [Route("api/candidates/search")]
    [ApiController]
    public class CandidatesSearchController : ControllerBase
    {
        private readonly ICandidatesSearchClient candidatesSearchClient;

        public CandidatesSearchController(ICandidatesSearchClient candidatesSearchClient)
        {
            this.candidatesSearchClient = candidatesSearchClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Candidate>> Get(
            [FromQuery] string searchText = null,
            [FromQuery] string filter = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] IList<string> orderBy = null,
            [FromQuery] IList<string> searchParameters = null)
        {
            var searchResults = this.candidatesSearchClient.SearchDocuments(searchText, filter, page, pageSize, orderBy, searchParameters);

            return Ok(searchResults.Select(s => s.Document));
        }

        [HttpPost]
        public void Post([FromBody] Candidate candidate)
        {
            this.candidatesSearchClient.InsertOrUpdateCandidates(candidate.ToList());
        }

        [HttpPost("activateIndexer")]
        public void ActivateIndexer()
        {
            
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            this.candidatesSearchClient.DeleteCandidates((id.CreateCandidate().ToList()));
        }
    }
}
