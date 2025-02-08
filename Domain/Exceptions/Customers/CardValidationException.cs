namespace Domain.Exceptions.Customers;

public class CardValidationException : DomainException
{
    public CardValidationException(String message) : base(message)
    {
    }
} 