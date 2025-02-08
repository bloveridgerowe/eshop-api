using Domain.Aggregates.Orders;
using Domain.Exceptions.Orders;
using Infrastructure.Persistence.Entities.Order;

namespace Infrastructure.Persistence.Mappers;

public static class OrderMapper
{
    public static Order ToDomain(this OrderEntity entity)
    {
        Order order = new Order(entity.Id, entity.CustomerId, entity.CreatedAt);
        
        order.AddItems(entity.Items.Select(ei => new OrderItem(ei.ProductId, ei.Price, ei.Quantity, ei.Product.Name)).ToList());

        if (!Enum.TryParse(entity.Status, out OrderProcessingStatus status))
        {
            throw new InvalidOrderStatusException(entity.Status);
        }
        
        order.SetStatus(status);

        return order;
    }

    public static OrderEntity ToPersistence(this Order domain)
    {
        return new OrderEntity
        {
            Id = domain.Id,
            CustomerId = domain.CustomerId,
            Status = domain.Status.ToString(),
            Items = domain.Items.Select(item => item.ToPersistence(domain.Id)).ToList(),
            CreatedAt = domain.CreatedAt
        };
    }
}