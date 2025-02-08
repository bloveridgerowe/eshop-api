namespace Domain.Exceptions.Customers;

public class EmailValidationException : Exception
{
    public EmailValidationException(String message) 
        : base(message)
    {
    }
}