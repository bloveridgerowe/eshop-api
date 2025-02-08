using MediatR;

namespace Application.Queries.Orders;

public class GetOrdersQuery : IRequest<GetOrdersQueryResponse>
{
    public Guid CustomerId { get; init; }
} 