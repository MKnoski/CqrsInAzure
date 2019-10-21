using System.Threading.Tasks;
using CqrsInAzure.Candidates.Models;
using CqrsInAzure.Candidates.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CqrsInAzure.Candidates.Controllers
{
    [Route("api/statusCheck")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestRepository requestRepository;

        public RequestsController(IRequestRepository requestRepository)
        {
            this.requestRepository = requestRepository;
        }
        
        [HttpGet("{correlationId}")]
        public async Task<ActionResult<Request>> StatusCheck([FromRoute] string correlationId)
        {
            if (string.IsNullOrEmpty(correlationId))
            {
                BadRequest();
            }

            return Ok(await this.requestRepository.GetItemAsync(correlationId, correlationId));
        }
    }
}