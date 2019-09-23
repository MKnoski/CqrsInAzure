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
                .Select(async s => await DeserializeCategoryAsync(s))
                .Select(t => t.Result);

            return deserializeCategories;
        }

        public async Task<bool> IsEmptyAsync()
        {
            return await this.storage.IsEmptyAsync();
        }

        public async Task<string> AddAsync(Category category)
        {
            if (category.Id == null)
            {
                category.Id = Guid.NewGuid().ToString();
            }

            using (var ms = new MemoryStream())
            {
                Utils.SerializeToJsonStream(ms, category);
                return await this.storage.UploadFileAsync(ms, category.Name, "application/json");
            }
        }

        public async Task<Category> GetAsync(string name)
        {
            var blob = this.storage.GetBlob(name);

            return await DeserializeCategoryAsync(blob);
        }

        public async Task DeleteAsync(string name)
        {
            await this.storage.DeleteFileAsync(name);
        }

        public async Task<string> UpdateAsync(string name, Category category)
        {
            var blob = this.storage.GetBlob(name);

            if (!await blob.ExistsAsync())
            {
                throw new FileNotFoundException();
            }

            if (category.Id == null)
            {
                category.Id = Guid.NewGuid().ToString();
            }

            using (var ms = new MemoryStream())
            {
                Utils.SerializeToJsonStream(ms, category);
                return await this.storage.ReUploadFileAsync(blob, category.Name ?? name, ms, "application/json");
            }
        }

        private async Task<Category> DeserializeCategoryAsync(CloudBlockBlob blob)
        {
            var json = await blob.DownloadTextAsync();
            var deserializedCategory = Newtonsoft.Json.JsonConvert.DeserializeObject<Category>(json);

            return deserializedCategory;
        }
    }
}
