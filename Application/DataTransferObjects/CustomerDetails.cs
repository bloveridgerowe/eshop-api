using Domain.ValueObjects;

namespace Application.DataTransferObjects;

public class CustomerDetails
{
    public String? Name { get; init; }
    public String Email { get; init; }
    public Address? Address { get; init; }
    public CardDetails? CardDetails { get; init; }
}