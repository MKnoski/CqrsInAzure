using System.Collections.Generic;
using CqrsInAzure.Candidates.Services;
using Microsoft.AspNetCore.Mvc;

namespace CqrsInAzure.Candidates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly CandidatesService _service;

        public CandidatesController()
        {
            _service = new CandidatesService();
        }

        // GET: api/Candidates
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Candidates/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Candidates
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Candidates/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
