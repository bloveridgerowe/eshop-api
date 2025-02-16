using Application.DataTransferObjects;
using Application.Queries.Baskets;

namespace Presentation.Api.Endpoints.Baskets;

public class GetBasketDetailsHttpResponse
{
    public BasketDetails BasketDetails { get; init; } = null!;

    public static GetBasketDetailsQueryResponse Empty => new();
}