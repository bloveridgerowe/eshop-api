namespace Domain.Exceptions.Customers;

public class AddressValidationException : Exception
{
    public AddressValidationException(String message) : base(message)
    {
    }
}