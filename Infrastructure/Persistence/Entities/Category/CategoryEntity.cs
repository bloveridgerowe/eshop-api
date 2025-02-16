using Infrastructure.Persistence.Entities.Product;

namespace Infrastructure.Persistence.Entities.Category;

public class CategoryEntity
{
    public Guid Id { get; set; }
    public String Name { get; set; }
    public ICollection<ProductEntity> Products { get; set; } = (List<ProductEntity>) [];
} 