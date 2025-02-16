using Domain.Aggregates.Basket;
using Infrastructure.Persistence.Entities.Basket;

namespace Infrastructure.Persistence.Mappers;

public static class BasketItemMapper
{
    public static BasketItem ToDomain(this BasketItemEntity basketItemEntity)
    {
        return new BasketItem(basketItemEntity.ProductId, basketItemEntity.Quantity, basketItemEntity.Price, basketItemEntity.Product.Name);
    }

    public static BasketItemEntity ToPersistence(this BasketItem basketItem, Guid basketId)
    {
        return new BasketItemEntity
        {
            Id = Guid.NewGuid(),
            BasketId = basketId,
            ProductId = basketItem.ProductId,
            Quantity = basketItem.Quantity,
            Price = basketItem.Price
        };
    }
}