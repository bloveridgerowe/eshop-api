using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence.Entities.Category;
using Infrastructure.Persistence.Entities.Product;
using Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ECommerceDbContext _context;

    public ProductRepository(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> FindByIdAsync(Guid id)
    {
        ProductEntity? entity = await _context.Products
            .Include(p => p.Categories)
            .SingleOrDefaultAsync(p => p.Id == id);

        return entity?.ToDomain();
    }


    
    public async Task<bool> DeleteAsync(Guid productId)
    {
        ProductEntity? existingEntity = await _context.Products
            .SingleOrDefaultAsync(p => p.Id == productId);

        if (existingEntity == null)
        {
            return false;
        }

        _context.Products.Remove(existingEntity);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task SaveAsync(Product product)
    {
        // Fetch all existing categories
        List<CategoryEntity> allCategoryEntities = await _context.Categories.ToListAsync();

        // Ensure any missing categories exist
        List<CategoryEntity> newCategories = product.Categories
            .Where(category => !allCategoryEntities.Any(c => c.Name == category))
            .Select(category => new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = category
            })
            .ToList();

        if (newCategories.Any())
        {
            await _context.Categories.AddRangeAsync(newCategories);
            allCategoryEntities.AddRange(newCategories);
        }

        // Filter down to the categories the product requires
        List<CategoryEntity> productCategoryEntities = allCategoryEntities
            .Where(c => product.Categories.Contains(c.Name))
            .ToList();

        // Fetch existing product
        ProductEntity? existingEntity = await _context.Products
            .Include(p => p.Categories)
            .SingleOrDefaultAsync(p => p.Id == product.Id);

        if (existingEntity == null)
        {
            // Add new product
            await _context.Products.AddAsync(product.ToPersistence(productCategoryEntities));
        }
        else
        {
            // Update existing product
            existingEntity.Name = product.Name;
            existingEntity.ImageUrl = product.ImageUrl;
            existingEntity.Featured = product.Featured;
            existingEntity.Price = product.Price;
            existingEntity.Stock = product.Stock;

            // Update product-category relationships
            // Add new categories
            foreach (CategoryEntity category in productCategoryEntities)
            {
                if (!existingEntity.Categories.Any(ec => ec.Name == category.Name))
                {
                    existingEntity.Categories.Add(category);
                }
            }

            // Remove categories no longer required
            List<CategoryEntity> categoriesToRemove = existingEntity.Categories
                .Where(ec => !product.Categories.Contains(ec.Name))
                .ToList();

            foreach (CategoryEntity categoryToRemove in categoriesToRemove)
            {
                existingEntity.Categories.Remove(categoryToRemove);
            }
        }

        // Changes saved using ECommerceUnitOfWork
    }

    public async Task SaveAsync(List<Product> products)
    {
        foreach (Product product in products)
        {
            await SaveAsync(product);
        }
    }
}