using Domain.Entities;

namespace Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> FindByIdAsync(Guid id);
    Task SaveAsync(Product product);
    Task SaveAsync(List<Product> products);
    Task<Boolean> DeleteAsync(Guid productId);
}