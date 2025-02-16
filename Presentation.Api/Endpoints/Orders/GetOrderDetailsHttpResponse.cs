using Application.DataTransferObjects;

namespace Presentation.Api.Endpoints.Orders;

public class GetOrderDetailsHttpResponse
{
    public OrderDetails? OrderDetails { get; init; }
    
}