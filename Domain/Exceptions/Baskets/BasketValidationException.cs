namespace Domain.Exceptions.Baskets;

public class BasketValidationException : DomainException
{
    public BasketValidationException(String message) : base(message)
    {
    }
} 