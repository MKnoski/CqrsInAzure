using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CqrsInAzure.Candidates.Storage
{
    public class PhotosStorage : IPhotosStorage
    {
        private readonly Storage storage;

        private static readonly string ContainerName = "photos";

        public PhotosStorage()
        {
            this.storage = new Storage(ContainerName);
        }

        public async Task<string> UploadAsync(Stream fileStream, string contentType)
        {
            var name = Guid.NewGuid().ToString();

            return await this.storage.UploadFileAsync(fileStream, name, contentType);
        }

        public string GetLink(string id)
        {
            return this.storage.GetDownloadLink(id);
        }

        public async Task<FileContentResult> GetAsync(string id)
        {
            return await this.storage.DownloadFileAsync(id);
        }

        public async Task DeleteAsync(string id)
        {
            await this.storage.DeleteFileAsync(id);
        }
    }
}
