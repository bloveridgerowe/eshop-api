using Application.Commands.Customers;

namespace Presentation.Api.Endpoints.Customers;

public class UpdateCustomerDetailsHttpRequest
{
    public String? Name { get; init; }
    public UpdateAddressDetails? Address { get; init; }
    public UpdateCardDetails? CardDetails { get; init; }
} 