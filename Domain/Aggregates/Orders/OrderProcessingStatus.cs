namespace Domain.Aggregates.Orders;

public enum OrderProcessingStatus
{
    Pending,
    Shipped,
    Delivered,
    Canceled
}
