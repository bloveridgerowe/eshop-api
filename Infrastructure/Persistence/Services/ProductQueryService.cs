using Application.DataTransferObjects;
using Application.Services;
using Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Services;

public class ProductQueryService : IProductQueryService
{
    private readonly ECommerceDbContext _context;

    public ProductQueryService(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductSummary>> FindFeaturedAsync()
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Where(p => p.Featured)
            .Select(p => p.ToQueryModel())
            .ToListAsync();
    }

    public async Task<List<ProductSummary>> FindByCategoryAsync(Guid categoryId)
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Where(p => p.Categories.Any(c => c.Id == categoryId))
            .Select(p => p.ToQueryModel())
            .ToListAsync();
    }

    public async Task<List<ProductSummary>> FindByPartialNameAsync(String partialName)
    {
        return await _context.Products
            .Include(p => p.Categories)
            .Where(p => p.Name.ToLower().Contains(partialName.ToLower()))
            .Select(entity => entity.ToQueryModel())
            .ToListAsync();
    }
}