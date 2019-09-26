using System.Collections.Generic;
using System.Threading.Tasks;
using CqrsInAzure.Categories.EventGrid;
using CqrsInAzure.Categories.EventGrid.Models;
using CqrsInAzure.Categories.Models;
using CqrsInAzure.Categories.Storage;
using Microsoft.AspNetCore.Mvc;

namespace CqrsInAzure.Categories.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesStorage storage;
        private readonly IEventPublisher eventPublisher;

        public CategoriesController(ICategoriesStorage storage, IEventPublisher eventPublisher)
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
            return await this.storage.GetAsync(name) ;
        }

        [HttpPost]
        public async Task<string> PostAsync([FromBody] Category category)
        {
            return await this.storage.AddAsync(category);
        }

        [HttpPut("{name}")]
        public async Task<string> Put(string name, [FromBody] Category category)
        {
            var newName = await this.storage.UpdateAsync(name, category);

            await this.eventPublisher.PublishCategoryUpdatedEventAsync(new CategoryUpdatedEventData { OldCategoryName = name, NewCategoryName = newName }
            );

            return newName;
        }

        [HttpDelete("{name}")]
        public async void DeleteAsync(string name)
        {
            await this.storage.DeleteAsync(name);
        }
    }
}
