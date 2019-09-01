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

        [HttpGet("{id}")]
        public async Task<Category> GetAsync(string id)
        {
            return await this.storage.GetAsync(id) ;
        }

        [HttpPost]
        public async void PostAsync([FromBody] Category category)
        {
            await this.storage.AddAsync(category);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
