using Domain.Aggregates.Orders;
using Infrastructure.Persistence.Entities.Order;

namespace Infrastructure.Persistence.Mappers;

public static class OrderItemMapper
{

    public static OrderItem ToDomain(this OrderItemEntity entity)
    {
        return new OrderItem(entity.ProductId, entity.Price, entity.Quantity, entity.Product.Name);
    }
        
    public static OrderItemEntity ToPersistence(this OrderItem domain, Guid orderId)
    {
        return new OrderItemEntity
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = domain.ProductId,
            Price = domain.Price,
            Quantity = domain.Quantity
        };
    }
}