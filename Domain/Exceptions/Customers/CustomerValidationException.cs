namespace Domain.Exceptions.Customers;

public class CustomerValidationException : DomainException
{
    public CustomerValidationException(String message) : base(message)
    {
    }
} 