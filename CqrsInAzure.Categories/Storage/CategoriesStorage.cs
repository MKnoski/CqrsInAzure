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

        private readonly Storage storage;

        public CategoriesStorage()
        {
            this.storage = new Storage(ContainerName);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var categories = await storage.GetAllAsync();

            var deserializeCategories = categories
                .Select(async s => await DeserilizeCategoryAsync(s))
                .Select(t => t.Result);

            return deserializeCategories;
        }

        public async Task<bool> IsEmptyAsync()
        {
            return await this.storage.IsEmptyAsync();
        }

        public Task<string> AddAsync(Category category)
        {
            if (category.Id == null)
            {
                category.Id = Guid.NewGuid().ToString();
            }

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

        public async Task<Category> GetAsync(string name)
        {
            var category = this.storage.Get(name);

            var deserializedCategory = await DeserilizeCategoryAsync(category);

            return deserializedCategory;
        }

        public async Task DeleteAsync(string name)
        {
            await this.storage.DeleteFileAsync(name);
        }

        public async Task<string> UpdateAsync(string name, Category category)
        {
            var blob = this.storage.Get(name);

            if (!await blob.ExistsAsync())
            {
                throw new FileNotFoundException();
            }

            if (category.Id == null)
            {
                category.Id = Guid.NewGuid().ToString();
            }

            var stream = Utils.SerializeToStream(category);
            return await this.storage.ReUploadFileAsync(blob, category.Name ?? name, stream);

        }
    }
}
