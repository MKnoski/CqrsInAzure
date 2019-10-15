using System.Collections.Generic;
using System.Threading.Tasks;
using CqrsInAzure.Categories.Publishers.EventGrid;
using CqrsInAzure.Categories.EventGrid.Models;
using CqrsInAzure.Categories.Models;
using CqrsInAzure.Categories.Storage;
using Microsoft.AspNetCore.Mvc;
using CqrsInAzure.Candidates.Helpers;

namespace CqrsInAzure.Categories.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesStorage storage;
        private readonly ICategoryEventPublisher eventPublisher;

        public CategoriesController(ICategoriesStorage storage, ICategoryEventPublisher eventPublisher)
        {
            this.storage = storage;
            this.eventPublisher = eventPublisher;
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
            return await this.storage.GetAsync(name);
        }

        [HttpPost]
        public async Task PostAsync([FromBody] Category category)
        {
            await this.storage.AddAsync(category);
        }

        [HttpPut("{name}")]
        public async Task PutAsync(string name, [FromBody] Category updateCategory)
        {
            var originalCategory = await this.storage.GetAsync(name);

            var newCategory = updateCategory.Merge(originalCategory);
            await this.storage.UpdateAsync(originalCategory.Name, newCategory);

            if (name != newCategory.Name)
            {
                await this.eventPublisher.PublishAsync(
                    "cqrsInAzure/category/updated",
                    new CategoryUpdatedEventData
                    {
                        OldCategoryName = name,
                        NewCategoryName = newCategory.Name
                    });
            }
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteAsync(string name)
        {
            var category = await this.storage.GetAsync(name);

            if (category.AssignedCandidates > 0)
            {
                return BadRequest("Category with assigned candidates cannot be deleted");
            }

            await this.storage.DeleteAsync(name);
            return Ok();
        }
    }
}