namespace Domain.Exceptions.Customers;

public class CustomerNotFoundException : DomainException
{
    public CustomerNotFoundException(Guid customerId) 
        : base($"Customer with ID {customerId} was not found")
    {
    }
}
