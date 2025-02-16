using Domain.Aggregates.Orders;

namespace Domain.Exceptions.Orders;

public class OrderStatusException : DomainException
{
    public OrderProcessingStatus CurrentStatus { get; }
    public OrderProcessingStatus AttemptedStatus { get; }
    
    public OrderStatusException(String message, OrderProcessingStatus currentStatus, OrderProcessingStatus attemptedStatus) 
        : base(message)
    {
        CurrentStatus = currentStatus;
        AttemptedStatus = attemptedStatus;
    }
} 