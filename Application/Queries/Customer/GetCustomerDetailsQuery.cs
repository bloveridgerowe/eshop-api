using MediatR;

namespace Application.Queries.Customer;

public class GetCustomerDetailsQuery : IRequest<GetCustomerDetailsQueryResponse>
{
    public Guid CustomerId { get; init; }
}