using Application.DataTransferObjects;
using Domain.Aggregates.Basket;

namespace Application.Mappers;

public static class BasketMapper
{
    public static BasketItemDetails ToCommandQueryModel(this BasketItem basketItem)
    {
        return new BasketItemDetails
        {
            Name = basketItem.Name,
            ProductId = basketItem.ProductId,
            Quantity = basketItem.Quantity,
            Price = basketItem.Price,
        };
    }

    public static BasketDetails ToCommandQueryModel(this Basket basket)
    {
        return new BasketDetails
        {
            Id = basket.Id,
            CustomerId = basket.CustomerId,
            CreatedAt = basket.CreatedAt,
            UpdatedAt = basket.UpdatedAt,
            TotalPrice = basket.TotalPrice,
            Items = basket.Items.Select(item => item.ToCommandQueryModel()).ToList()
        };
    }
}