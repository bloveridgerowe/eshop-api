using MediatR;

namespace Application.Queries.Orders;

public class GetOrderDetailsQuery : IRequest<GetOrderDetailsQueryResponse>
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
} 