using Domain.Aggregates.Orders;

namespace Domain.Events.Orders;

public class OrderStatusChangedEvent : Event
{
    public Guid OrderId { get; }
    public OrderProcessingStatus OrderStatus { get; }

    public OrderStatusChangedEvent(Guid orderId, OrderProcessingStatus orderStatus)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
    }
}