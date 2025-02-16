using Domain.Aggregates.Orders;

namespace Application.DataTransferObjects;

public class OrderDetails
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public List<OrderItemDetails> Items { get; init; } = [];
    public Decimal TotalPrice { get; init; }
    public OrderProcessingStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class OrderItemDetails
{
    public Guid ProductId { get; init; }
    public String Name { get; init; }
    public Decimal Price { get; init; }
    public Int32 Quantity { get; init; }
    public Decimal TotalPrice { get; init; }
} 