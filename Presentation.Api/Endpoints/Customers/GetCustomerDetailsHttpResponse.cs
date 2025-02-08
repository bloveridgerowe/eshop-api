using Domain.ValueObjects;

namespace Presentation.Api.Endpoints.Customers;

public class GetCustomerDetailsHttpResponse
{
    public SanitizedCustomerDetails? CustomerDetails { get; init; }
}

public class SanitizedCustomerDetails
{
    public String? Name { get; init; }
    public String Email { get; init; }
    public Address? Address { get; init; }
    public SanitizedCardDetails? CardDetails { get; init; }
}

public class SanitizedCardDetails
{
    public String CardNumber { get; init; }
    public String ExpiryDate { get; init; }
    public String Cvv { get; init; }
}