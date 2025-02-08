using Application.Commands.Orders;
using Application.Queries.Orders;
using Presentation.Api.Endpoints.Orders;

namespace Presentation.Api.Mappers;

public static class OrderMapper
{
    public static ConvertBasketToOrderHttpResponse ToHttpResponse(this ConvertBasketToOrderCommandResponse response)
    {
        return new ConvertBasketToOrderHttpResponse
        {
            OrderId = response.OrderId
        };
    }

    public static GetOrderDetailsHttpResponse ToHttpResponse(this GetOrderDetailsQueryResponse response)
    {
        return new GetOrderDetailsHttpResponse
        {
            OrderDetails = response.OrderDetails
        };
    }

    public static GetOrdersHttpResponse ToHttpResponse(this GetOrdersQueryResponse response)
    {
        return new GetOrdersHttpResponse
        {
            Orders = response.Orders
        };
    }
} 