using CqrsInAzure.Candidates.Repositories;
using CqrsInAzure.Candidates.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CqrsInAzure.Candidates.Controllers
{
    [Route("api/candidates/{candidateId}/{partitionKey}/[controller]")]
    public class CvController : Controller
    {
        private readonly ICandidatesRepository repository;
        private readonly ICvStorage cvStorage;

        public CvController(ICandidatesRepository repository, ICvStorage cvStorage)
        {
            this.repository = repository;
            this.cvStorage = cvStorage;
        }

        [HttpGet()]
        public async Task<ActionResult<byte[]>> GetAsync(string candidateId, string partitionKey)
        {
            var candidate = await this.repository.GetItemAsync(candidateId, partitionKey);

            if (candidate?.CvId == null)
                return null;

            return await this.cvStorage.GetAsync(candidate.CvId);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(string candidateId, string partitionKey, [FromForm] IFormFile file)
        {
            if (file.Length == 0)
                return BadRequest("Empty received from the upload");

            if (!IsSupported(file))
                return BadRequest("Invalid file received from the upload");

            try
            {
                using (Stream fileStream = file.OpenReadStream())
                {
                    var cvId = await this.cvStorage.UploadAsync(fileStream, file.ContentType);

                    var candidate = await this.repository.GetItemAsync(candidateId, partitionKey);
                    candidate.CvId = cvId;

                    await this.repository.UpdateItemAsync(candidateId, partitionKey, candidate);

                    return Ok(cvId);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete()]
        public async Task<ActionResult> DeleteAsync(string candidateId, string partitionKey)
        {
            var candidate = await this.repository.GetItemAsync(candidateId, partitionKey);

            if (candidate?.CvId == null)
                return NoContent();

            await this.cvStorage.DeleteAsync(candidate.CvId);

            candidate.CvId = null;

            await this.repository.UpdateItemAsync(candidateId, partitionKey, candidate);

            return Ok();
        }

        public static bool IsSupported(IFormFile file)
        {
            if (file.ContentType.Contains("application/pdf"))
            {
                return true;
            }

            string[] formats = new string[] { ".pdf", ".docx" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}
