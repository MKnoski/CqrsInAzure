﻿using CqrsInAzure.Categories.Models;
using CqrsInAzure.Categories.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CqrsInAzure.Categories
{
    public class DataSeeder
    {
        public static async Task Seed(ICategoriesStorage storage)
        {
            if (await storage.IsEmptyAsync())
            {
                foreach (var category in Categories)
                {
                    await storage.AddAsync(category);
                }
            }
        }

        private static IEnumerable<Category> Categories = new List<Category>
        {
            new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Education",
                SortOrder = SortOrder.Medium
            },
            new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "SoftwareDevelopment",
                SortOrder = SortOrder.High
            },
            new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "RealEstate",
                SortOrder = SortOrder.Low
            },
        };
    }
}