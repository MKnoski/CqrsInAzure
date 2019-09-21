using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace CqrsInAzure.Categories.Storage
{
    public static class StorageExtensions
    {
        public static string GetDownloadLink(this CloudBlobContainer container, string blobName)
        {
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

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
    }
}
