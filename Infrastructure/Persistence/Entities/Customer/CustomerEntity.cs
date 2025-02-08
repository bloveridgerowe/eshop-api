namespace Infrastructure.Persistence.Entities.Customer;

public class CustomerEntity
{
    public Guid Id { get; set; }
    public String? Name { get; set; }
    public String Email { get; set; }
    public CustomerAddressEntity? Address { get; set; }
    public CustomerCardDetailsEntity? CardDetails { get; set; }
}