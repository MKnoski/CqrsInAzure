using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CqrsInAzure.Candidates.Storage
{
    public interface ICvStorage
    {
        Task<string> UploadAsync(Stream fileStream, string contentType);
        string GetLink(string id);
        Task<FileContentResult> GetAsync(string id);
        Task DeleteAsync(string id);
    }
}