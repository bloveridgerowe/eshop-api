using System.Security.Claims;
using Application.Commands.Baskets;
using Application.Commands.Orders;
using Application.Queries.Baskets;
using Infrastructure.Extensions;
using Presentation.Api.Endpoints.Baskets;
using Presentation.Api.Extensions;

namespace Presentation.Api.Mappers;

public static class BasketMapper
{
    public static GetBasketDetailsHttpResponse ToHttpResponse(this GetBasketDetailsQueryResponse response)
    {
        return new GetBasketDetailsHttpResponse
        {
            BasketDetails = response.BasketDetails
        };
    }

    public static UpdateBasketDetailsCommand ToCommand(this UpdateBasketDetailsHttpRequest command, ClaimsPrincipal user)
    {
        return new UpdateBasketDetailsCommand
        {
            CustomerId = user.GetId(),
            Items = command.Items
        };
    }
    
} 