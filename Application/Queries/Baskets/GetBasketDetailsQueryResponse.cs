using Application.DataTransferObjects;

namespace Application.Queries.Baskets;

public class GetBasketDetailsQueryResponse
{
    public BasketDetails BasketDetails { get; init; } = null!;

    public static GetBasketDetailsQueryResponse Empty => new();
}