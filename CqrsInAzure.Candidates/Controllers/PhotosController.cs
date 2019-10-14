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
    public class PhotosController : Controller
    {
        private readonly ICandidatesRepository repository;
        private readonly IPhotosStorage photosStorage;

        public PhotosController(ICandidatesRepository repository, IPhotosStorage photosStorage)
        {
            this.repository = repository;
            this.photosStorage = photosStorage;
        }

        [HttpGet()]
        public async Task<ActionResult<byte[]>> GetAsync(string candidateId, string partitionKey)
        {
            var candidate = await this.repository.GetItemAsync(candidateId, partitionKey);

            if (candidate?.PhotoId == null)
                return null;

            return await this.photosStorage.GetAsync(candidate.PhotoId);
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
                    var photoId = await this.photosStorage.UploadAsync(fileStream, file.ContentType);

                    var candidate = await this.repository.GetItemAsync(candidateId, partitionKey);
                    candidate.PhotoId = photoId;

                    await this.repository.UpdateItemAsync(candidateId, partitionKey, candidate);

                    return Ok(photoId);
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

            if (candidate?.PhotoId == null)
                return NoContent();

            await this.photosStorage.DeleteAsync(candidate.PhotoId);

            candidate.PhotoId = null;

            await this.repository.UpdateItemAsync(candidateId, partitionKey, candidate);

            return Ok();
        }

        public static bool IsSupported(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}