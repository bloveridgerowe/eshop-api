using MediatR;

namespace Application.Commands.Orders;

public class ConvertBasketToOrderCommand : IRequest<ConvertBasketToOrderCommandResponse>
{
    public Guid CustomerId { get; init; }
    public Guid BasketId { get; init; }
} 