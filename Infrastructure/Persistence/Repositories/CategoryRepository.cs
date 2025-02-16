using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence.Entities.Category;
using Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ECommerceDbContext _dbContext;

    public CategoryRepository(ECommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _dbContext.Categories.Select(c => c.ToDomain()).ToListAsync();
    }

    public async Task SaveAsync(Category category)
    {
        // Find the existing category by ID
        CategoryEntity? existingCategory = await _dbContext.Categories.FindAsync(category.Id);

        if (existingCategory == null)
        {
            // Add a new category if it doesn't exist
            await _dbContext.Categories.AddAsync(category.ToPersistence());
        }
        else
        {
            // Update the existing category
            existingCategory.Name = category.Name;
        }

        // Changes saved using ECommerceUnitOfWork
    }
}