namespace Domain.Exceptions.Orders;

public class OrderValidationException : DomainException
{
    public OrderValidationException(String message) : base(message)
    {
    }
} 