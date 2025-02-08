using MediatR;

namespace Application.Commands.Customers;

public class UpdateCustomerDetailsCommand : IRequest
{
    public Guid CustomerId { get; init; }
    public String? Name { get; init; }
    public UpdateAddressDetails? Address { get; init; }
    public UpdateCardDetails? CardDetails { get; init; }
}

public class UpdateAddressDetails
{
    public String FirstLine { get; init; }
    public String SecondLine { get; init; }
    public String City { get; init; }
    public String County { get; init; }
    public String PostCode { get; init; }
}

public class UpdateCardDetails
{
    public String CardNumber { get; init; }
    public String Cvv { get; init; }
    public String ExpiryDate { get; init; }
}