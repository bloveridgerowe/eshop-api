using Application.Queries.Baskets;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Baskets;

[HttpGet("/me/basket")]
[Authorize]
public class GetBasketDetailsEndpointHandler : EndpointWithoutRequest<GetBasketDetailsHttpResponse>
{
    private readonly IMediator _mediator;

    public GetBasketDetailsEndpointHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        GetBasketDetailsQuery getBasketDetailsQuery = new GetBasketDetailsQuery
        {
            CustomerId = User.GetId()
        };
        
        GetBasketDetailsQueryResponse basketDetailsQueryResponse = await _mediator.Send(getBasketDetailsQuery, cancellationToken);
        await SendOkAsync(basketDetailsQueryResponse.ToHttpResponse(), cancellationToken);
    }
} 