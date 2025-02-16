using Domain.Aggregates.Basket;
using Infrastructure.Persistence.Entities.Basket;

namespace Infrastructure.Persistence.Mappers;

public static class BasketMapper
{
    public static BasketEntity ToPersistence(this Basket basket)
    {
        return new BasketEntity
        {
            Id = basket.Id,
            CustomerId = basket.CustomerId,
            CreatedAt = basket.CreatedAt,
            UpdatedAt = basket.UpdatedAt,
            Items = basket.Items.Select(basketItem => basketItem.ToPersistence(basket.Id)).ToList()
        };
    }

    public static Basket ToDomain(this BasketEntity basketEntity)
    {
        Basket basket = new Basket(basketEntity.Id, basketEntity.CustomerId, basketEntity.CreatedAt, basketEntity.UpdatedAt);

        if (basketEntity.Items.Any())
        {
            basket.AddItems(basketEntity.Items.Select(i => i.ToDomain()));
        }

        return basket;
    }
}