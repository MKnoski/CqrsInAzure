using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CqrsInAzure.Categories.Storage
{
    public class Storage
    {
        private readonly CloudBlobContainer container;

        // move to settings
        private readonly string AccountName = "cqrsinazure";
        private readonly string AccountKey = "XgcCDsrFhhdD9Tf0seCVJIBqd3NioaGdJ1LNv7ufMTqtHBTRTKdewctwDNs+0BhCH5IjFB1XY+KVlrJ1qeaOZQ==";

        public Storage(string containerName)
        {
            StorageCredentials storageCredentials = new StorageCredentials(AccountName, AccountKey);

            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            container = blobClient.GetContainerReference(containerName);

            container.CreateIfNotExistsAsync().Wait();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string name, string contentType = "")
        {
            CloudBlockBlob blob = GetBlob(name);

            await blob.UploadFromStreamAsync(fileStream);

            if (!string.IsNullOrEmpty(contentType))
            {
                blob.Properties.ContentType = contentType;
                await blob.SetPropertiesAsync();
            }

            return blob.Name;
        }
        
        public async Task<string> ReUploadFileAsync(CloudBlockBlob oldBlob, string newName, Stream fileStream, string contentType = "")
        {
            var name = await UploadFileAsync(fileStream, newName, contentType);

            if (oldBlob.Name != newName)
            {
                await oldBlob.DeleteIfExistsAsync();
            }

            return name;
        }

        public async Task DeleteFileAsync(string name)
        {
            CloudBlockBlob blob = GetBlob(name);

            await blob.DeleteIfExistsAsync();
        }

        public CloudBlockBlob GetBlob(string name)
        {
            CloudBlockBlob blob = container.GetBlockBlobReference(name);

            return blob;
        }

        public async Task<IEnumerable<CloudBlockBlob>> GetAllAsync()
        {
            BlobContinuationToken blobContinuationToken = null;
            var results = await this.container.ListBlobsSegmentedAsync(null, blobContinuationToken);

            return results.Results.Select(s => s as CloudBlockBlob);
        }

        public async Task<bool> IsEmptyAsync()
        {
            var files = await GetAllAsync();

            return !files.Any();
        }
    }
}
