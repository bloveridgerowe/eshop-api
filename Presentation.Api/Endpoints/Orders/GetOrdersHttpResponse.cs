using Application.DataTransferObjects;

namespace Presentation.Api.Endpoints.Orders;

public class GetOrdersHttpResponse
{
    public List<OrderDetails> Orders { get; init; } = [];
}