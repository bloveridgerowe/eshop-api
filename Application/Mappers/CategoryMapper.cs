using Application.DataTransferObjects;
using Domain.Entities;

namespace Application.Mappers;

public static class CategoryMapper
{
    public static CategoryDetails ToQueryModel(this Category category)
    {
        return new CategoryDetails
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}