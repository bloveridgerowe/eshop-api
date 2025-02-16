using Application.Queries.Baskets;
using FastEndpoints;
using Infrastructure.Extensions;
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
        GetBasketDetailsQueryResponse basketDetailsQueryResponse = await _mediator.Send(new GetBasketDetailsQuery
        {
            CustomerId = User.GetId() 
        });
        
        await SendOkAsync(basketDetailsQueryResponse.ToHttpResponse());
    }
} 