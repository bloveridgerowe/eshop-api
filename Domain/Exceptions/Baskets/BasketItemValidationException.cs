namespace Domain.Exceptions.Baskets;

public class BasketItemValidationException : DomainException
{
    public BasketItemValidationException(String message) : base(message)
    {
    }
} 