namespace Infrastructure.Persistence.Entities.Customer;

public class CustomerAddressEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public String FirstLine { get; set; }
    public String SecondLine { get; set; }
    public String City { get; set; }
    public String County { get; set; }
    public String PostCode { get; set; }
}