using MediatR;

namespace Application.Queries.Customers;

public class GetCustomerDetailsQuery : IRequest<GetCustomerDetailsQueryResponse>
{
    public Guid CustomerId { get; init; }
}