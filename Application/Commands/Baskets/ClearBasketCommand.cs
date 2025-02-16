using MediatR;

namespace Application.Commands.Baskets;

public class ClearBasketCommand : IRequest
{
    public Guid CustomerId { get; init; }
}