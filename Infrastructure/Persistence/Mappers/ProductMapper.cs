using Domain.Entities;
using Infrastructure.Persistence.Entities.Category;
using Infrastructure.Persistence.Entities.Product;

namespace Infrastructure.Persistence.Mappers;

public static class ProductMapper
{
    public static Product ToDomain(this ProductEntity entity)
    {
        return new Product(entity.Id, entity.Name, entity.Featured, entity.ImageUrl, entity.Price, entity.Stock, entity.Categories.Select(c => c.Name).ToList());
    }

    // categories are essentially tags on a product, to allow for convenient grouping and filtering
    // the rich domain model should not have any knowledge of category ids, they're just a persistence detail
    // as such we need to resolve the categories in the repository layer, and pass them into here for mapping
    public static ProductEntity ToPersistence(this Product domain, List<CategoryEntity> resolvedCategories)
    {
        return new ProductEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Price = domain.Price,
            Stock = domain.Stock,
            ImageUrl = domain.ImageUrl,
            Featured = domain.Featured,
            Categories = resolvedCategories
        };
    }
}