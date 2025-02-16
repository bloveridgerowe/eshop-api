using Application.DataTransferObjects;
using Domain.Aggregates.Orders;

namespace Application.Mappers;

public static class OrderMapper
{
    public static OrderDetails ToQueryModel(this Order order)
    {
        return new OrderDetails
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            TotalPrice = order.TotalPrice,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(item => item.ToQueryModel()).ToList()
        };
    }

    public static OrderItemDetails ToQueryModel(this OrderItem orderItem)
    {
        return new OrderItemDetails
        {
            ProductId = orderItem.ProductId,
            Name = orderItem.Name,
            Quantity = orderItem.Quantity,
            Price = orderItem.Price,
            TotalPrice = orderItem.TotalPrice
        };
    }
}