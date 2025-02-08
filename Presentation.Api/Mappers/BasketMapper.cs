using Application.Queries.Baskets;
using Presentation.Api.Endpoints.Baskets;

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
} 