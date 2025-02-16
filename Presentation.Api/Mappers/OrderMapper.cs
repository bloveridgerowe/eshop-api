using System.Security.Claims;
using Application.Commands.Orders;
using Application.Queries.Orders;
using Infrastructure.Extensions;
using Presentation.Api.Endpoints.Orders;
using Presentation.Api.Extensions;

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

    public static GetOrderDetailsQuery ToQuery(this GetOrderDetailsHttpRequest request, ClaimsPrincipal user)
    {
        return new GetOrderDetailsQuery
        {
            OrderId = request.OrderId,
            CustomerId = user.GetId()
        };
    }
} 