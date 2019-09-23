using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
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
            CloudBlockBlob blob = Get(name);

            fileStream.Position = 0;
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
            await oldBlob.DeleteIfExistsAsync();

            return name;
        }

        public async Task DeleteFileAsync(string name)
        {
            CloudBlockBlob blob = Get(name);

            await blob.DeleteIfExistsAsync();
        }

        public CloudBlockBlob Get(string name)
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

        #region Download files

        public string GetDownloadLink(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                return string.Empty;
            }

            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(12),
            };

            var sasToken = blob.GetSharedAccessSignature(policy);

            return blob.Uri.AbsoluteUri + sasToken;
        }

        public async Task<byte[]> DownloadFileAsync(string name)
        {
            CloudBlockBlob blob = Get(name);

            await blob.FetchAttributesAsync();
            long fileByteLength = blob.Properties.Length;
            byte[] byteArray = new Byte[fileByteLength];

            await blob.DownloadToByteArrayAsync(byteArray, 0);

            return byteArray;
        }

        private async Task DownloadToFile(string name)
        {
            CloudBlockBlob blob = Get(name);

            string localPath = Path.GetTempFileName();

            await blob.DownloadToFileAsync(localPath, FileMode.Create);
        }

        private async Task<string> GetBlobName(string uri)
        {
            var blob = await container.ServiceClient.GetBlobReferenceFromServerAsync(new Uri(uri));

            return blob.Name;
        }

        #endregion
    }
}
