using Application.DataTransferObjects;
using Domain.Entities;

namespace Application.Mappers;

public static class ProductMapper
{
    public static ProductSummary ToSummaryQueryModel(this Product product)
    {
        return new ProductSummary
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Featured = product.Featured,
            ImageUrl = product.ImageUrl,
            Categories = product.Categories
        };
    }
    
    public static ProductDetails ToDetailsQueryModel(this Product product)
    {
        return new ProductDetails
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Featured = product.Featured,
            ImageUrl = product.ImageUrl,
            Categories = product.Categories
        };
    }
}