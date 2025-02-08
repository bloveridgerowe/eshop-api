using Application.DataTransferObjects;

namespace Application.Queries.Orders;

public class GetOrdersQueryResponse
{
    public List<OrderDetails> Orders { get; init; } = [];
} 