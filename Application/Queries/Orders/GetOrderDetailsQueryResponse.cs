using Application.DataTransferObjects;

namespace Application.Queries.Orders;

public class GetOrderDetailsQueryResponse
{
    public OrderDetails? OrderDetails { get; init; }

    public static GetOrderDetailsQueryResponse Empty => new();
} 