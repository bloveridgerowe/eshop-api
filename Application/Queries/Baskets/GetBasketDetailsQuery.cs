using MediatR;

namespace Application.Queries.Baskets;

public class GetBasketDetailsQuery : IRequest<GetBasketDetailsQueryResponse>
{
    public Guid CustomerId { get; init; }
} 