using Application.Commands.Baskets;

namespace Presentation.Api.Endpoints.Baskets;

public class UpdateBasketDetailsHttpRequest
{
    public List<UpdateBasketItemDetails> Items { get; init; } = [];
} 