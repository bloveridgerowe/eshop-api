namespace Domain.Exceptions.Orders;

public class InvalidOrderStatusException : DomainException
{
    public InvalidOrderStatusException(Int32 orderStatusCode)
        : base($"Invalid order status: '{orderStatusCode}'")
    {
    }
    
    public InvalidOrderStatusException(String orderStatus)
        : base($"Invalid order status: '{orderStatus}'")
    {
    }
}