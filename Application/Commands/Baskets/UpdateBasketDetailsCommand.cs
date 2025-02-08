using MediatR;

namespace Application.Commands.Baskets;

public class UpdateBasketDetailsCommand : IRequest
{
    public Guid CustomerId { get; init; }
    public List<UpdateBasketItemDetails> Items { get; init; } = [];
}

public class UpdateBasketItemDetails
{
    public Guid ProductId { get; init; }
    public Int32 Quantity { get; init; }
}