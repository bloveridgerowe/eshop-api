using Infrastructure.Persistence.Entities.Category;

namespace Infrastructure.Persistence.Entities.Product;

public class ProductEntity
{
    public Guid Id { get; set; }
    public String Name { get; set; }
    public String Description { get; set; }
    public Decimal Price { get; set; } 
    public Int32 Stock { get; set; }
    public String ImageUrl { get; set; }
    public Boolean Featured { get; set; }
    public ICollection<CategoryEntity> Categories { get; set; } = (List<CategoryEntity>) [];
}