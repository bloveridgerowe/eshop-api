using Domain.Entities;

namespace Domain.Repositories;

public interface IProductRepository
{
    Task<List<Product>> FindFeaturedAsync();
    Task<List<Product>> FindByCategoryAsync(Guid categoryId);
    Task<Product?> FindByIdAsync(Guid id);
    Task<List<Product>> FindByPartialNameAsync(String partialName);
    Task SaveAsync(Product product);
    Task SaveAsync(List<Product> products);
    Task<Boolean> DeleteAsync(Guid productId);
}