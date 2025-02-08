using Domain.Entities;
using Infrastructure.Persistence.Entities.Category;

namespace Infrastructure.Persistence.Mappers;

public static class CategoryMapper
{
    public static CategoryEntity ToPersistence(this Category category)
    {
        return new CategoryEntity
        {
            Id = category.Id,
            Name = category.Name,
        };
    }

    public static Category ToDomain(this CategoryEntity categoryEntity)
    {
        return new Category(categoryEntity.Id, categoryEntity.Name);
    }
}