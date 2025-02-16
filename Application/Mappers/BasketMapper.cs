using Application.DataTransferObjects;
using Domain.Aggregates.Basket;

namespace Application.Mappers;

public static class BasketMapper
{
    public static BasketItemDetails ToQueryModel(this BasketItem basketItem)
    {
        return new BasketItemDetails
        {
            Name = basketItem.Name,
            ProductId = basketItem.ProductId,
            Quantity = basketItem.Quantity,
            Price = basketItem.Price,
        };
    }

    public static BasketDetails ToQueryModel(this Basket basket)
    {
        return new BasketDetails
        {
            Id = basket.Id,
            CustomerId = basket.CustomerId,
            CreatedAt = basket.CreatedAt,
            UpdatedAt = basket.UpdatedAt,
            TotalPrice = basket.TotalPrice,
            Items = basket.Items.Select(item => item.ToQueryModel()).ToList()
        };
    }
}