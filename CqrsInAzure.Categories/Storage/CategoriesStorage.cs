using CqrsInAzure.Candidates.Services;
using CqrsInAzure.Categories.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CqrsInAzure.Categories.Storage
{
    public class CategoriesStorage : ICategoriesStorage
    {
        private static readonly string ContainerName = "categories";

        private readonly Candidates.Services.Storage storage;

        public CategoriesStorage()
        {
            this.storage = new Candidates.Services.Storage(ContainerName);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var categories = await storage.ListAllFilesAsync();

            var deserializeCategories = categories
                .Select(async s => await DeserilizeCategoryAsync(s))
                .Select(t => t.Result);

            return deserializeCategories;
        }

        public async Task<bool> IsEmptyAsync()
        {
            return await this.storage.IsEmptyAsync();
        }

        public Task AddAsync(Category category)
        {
            var stream = Utils.SerializeToStream(category);
            return this.storage.UploadFileAsync(stream, category.Name);
        }

        private async Task<Category> DeserilizeCategoryAsync(CloudBlockBlob blob)
        {
            using (MemoryStream memstream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memstream);

                var category = Utils.DeserializeFromStream<Category>(memstream);

                return category;
            }
        }

        public async Task<Category> GetAsync(string id)
        {
            var category = (await GetAllAsync()).SingleOrDefault(c => c.Id == id);

            return category;
        }
    }
}
