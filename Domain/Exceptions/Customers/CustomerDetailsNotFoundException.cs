namespace Domain.Exceptions.Customers;

public class CustomerDetailsNotFoundException : Exception
{
    public Guid CustomerId { get; }

    public CustomerDetailsNotFoundException(Guid customerId)
        : base($"The customer {customerId} is missing the required address or card details.")
    {
        CustomerId = customerId;
    }
} 