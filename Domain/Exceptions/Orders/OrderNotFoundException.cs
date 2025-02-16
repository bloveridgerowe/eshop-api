namespace Domain.Exceptions.Orders;

public class OrderNotFoundException : DomainException
{
    public OrderNotFoundException(Guid orderId) 
        : base($"Order with ID {orderId} was not found")
    {
    }
}
