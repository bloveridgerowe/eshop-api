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
        order.SetStatus(OrderProcessingStatus.Statuses.Single(s => s.Id == entity.StatusId));

        return order;
    }

    public static OrderEntity ToPersistence(this Order domain)
    {
        return new OrderEntity
        {
            Id = domain.Id,
            CustomerId = domain.CustomerId,
            StatusId = domain.Status.Id,
            Items = domain.Items.Select(item => item.ToPersistence(domain.Id)).ToList(),
            CreatedAt = domain.CreatedAt
        };
    }
}