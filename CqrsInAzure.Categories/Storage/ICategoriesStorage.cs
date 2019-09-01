using CqrsInAzure.Categories.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CqrsInAzure.Categories.Storage
{
    public interface ICategoriesStorage
    {
        Task AddAsync(Category category);

        Task<bool> IsEmptyAsync();

        Task<IEnumerable<Category>> GetAllAsync();

        Task<Category> GetAsync(string id);
    }
}
