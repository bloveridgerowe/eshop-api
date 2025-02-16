namespace Infrastructure.Persistence.Entities.Customer;

public class CustomerCardDetailsEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public String CardNumber { get; set; }
    public String ExpiryDate { get; set; }
    public String Cvv { get; set; }
}