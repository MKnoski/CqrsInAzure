using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CqrsInAzure.Candidates.Services
{
    public class StorageService
    {
        private readonly CloudBlobContainer _container;

        private readonly string ContainerName = "";

        // move to settings
        private readonly string AccountName = "cqrsinazure";
        private readonly string AccountKey = "XgcCDsrFhhdD9Tf0seCVJIBqd3NioaGdJ1LNv7ufMTqtHBTRTKdewctwDNs+0BhCH5IjFB1XY+KVlrJ1qeaOZQ==";
        
        public StorageService()
        {
            StorageCredentials storageCredentials = new StorageCredentials(AccountName, AccountKey);

            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            var containerName = ContainerName;
            _container = blobClient.GetContainerReference(containerName);

            // await container.CreateIfNotExistsAsync();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType = "")
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var blobName = $"{fileName}_{timestamp}";

            CloudBlockBlob blob = _container.GetBlockBlobReference(blobName);

            await blob.UploadFromStreamAsync(fileStream);

            if (!string.IsNullOrEmpty(contentType))
            {
                blob.Properties.ContentType = contentType;
                await blob.SetPropertiesAsync();
            }

            return blob.Name;
        }

        public async Task<byte[]> DownloadFileAsync(string blobName)
        {
            var byteArray = await DownloadToByteArray(blobName);
            return byteArray;
        }

        public async Task DeleteFileAsync(string blobName)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(blobName);

            await blob.DeleteIfExistsAsync();
        }

        private async Task<byte[]> DownloadToByteArray(string blobName)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(blobName);

            var downloadLink = GetDownloadLinkAsync(blob.Name);

            await blob.FetchAttributesAsync();
            long fileByteLength = blob.Properties.Length;
            byte[] byteArray = new Byte[fileByteLength];

            await blob.DownloadToByteArrayAsync(byteArray, 0);

            return byteArray;
        }

        private async Task DownloadToFile(string blobName)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(blobName);

            string localPath = Path.GetTempFileName();

            await blob.DownloadToFileAsync(localPath, FileMode.Create);
        }

        private async Task<string> GetDownloadLinkAsync(string blobName)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(blobName);

            //Create an ad-hoc Shared Access Policy with read permissions which will expire in 12 hours
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(12),
            };

            //Set content-disposition header for force download
            SharedAccessBlobHeaders headers = new SharedAccessBlobHeaders()
            {
                ContentDisposition = string.Format("attachment;filename=\"{0}\"", blobName),
            };

            var sasToken = blob.GetSharedAccessSignature(policy, headers);
            return blob.Uri.AbsoluteUri + sasToken;
        }

        private async Task<string> GetBlobName(string uri)
        {
            var blob = await _container.ServiceClient.GetBlobReferenceFromServerAsync(new Uri(uri));

            return blob.Name;
        }
    }
}
