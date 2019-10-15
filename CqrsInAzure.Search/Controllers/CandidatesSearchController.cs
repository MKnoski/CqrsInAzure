using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult<IEnumerable<Candidate>>> SearchAsync(
            [FromQuery] string searchText = null,
            [FromQuery] string filter = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] IList<string> orderBy = null,
            [FromQuery] IList<string> searchParameters = null,
            [FromQuery] IList<string> searchFields = null)
        {
            var searchResults = await this.candidatesSearchClient.SearchDocumentsAsync(searchText, filter, page, pageSize, orderBy, searchParameters, searchFields);

            return Ok(searchResults.Select(s => s.Document));
        }

        [HttpPost]
        public async Task PostAsync([FromBody] Candidate candidate)
        {
            await this.candidatesSearchClient.InsertOrUpdateCandidatesAsync(candidate.ToList());
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            await this.candidatesSearchClient.DeleteCandidatesAsync((id.CreateCandidate().ToList()));
        }
    }
}