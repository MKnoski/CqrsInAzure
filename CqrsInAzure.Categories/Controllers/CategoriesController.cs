using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CqrsInAzure.Categories.Models;
using CqrsInAzure.Categories.Storage;
using Microsoft.AspNetCore.Mvc;

namespace CqrsInAzure.Categories.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ICategoriesStorage storage;

        public CategoriesController(ICategoriesStorage storage)
        {
            this.storage = storage;
        }

        [HttpGet]
        public async Task<IEnumerable<Category>> GetAsync()
        {
            var categories = await this.storage.GetAllAsync();

            return categories;
        }

        [HttpGet("{name}")]
        public async Task<Category> GetAsync(string name)
        {
            return await this.storage.GetAsync(name) ;
        }

        [HttpPost]
        public async Task<string> PostAsync([FromBody] Category category)
        {
            return await this.storage.AddAsync(category);
        }

        [HttpPut("{name}")]
        public Task<string> Put(string name, [FromBody] Category category)
        {
            return this.storage.UpdateAsync(name, category);
        }

        [HttpDelete("{name}")]
        public async void DeleteAsync(string name)
        {
            await this.storage.DeleteAsync(name);
        }
    }
}
