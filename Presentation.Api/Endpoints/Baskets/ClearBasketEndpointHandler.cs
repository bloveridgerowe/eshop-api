using Application.Commands.Baskets;
using FastEndpoints;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;

namespace Presentation.Api.Endpoints.Baskets;

[HttpDelete("/me/basket")]
[Authorize]
public class ClearBasketEndpointHandler : EndpointWithoutRequest<GetBasketDetailsHttpResponse>
{
    private readonly IMediator _mediator;

    public ClearBasketEndpointHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        ClearBasketCommand clearBasketCommand = new ClearBasketCommand
        {
            CustomerId = User.GetId()
        };
        
        await _mediator.Send(clearBasketCommand, cancellationToken);
        await SendOkAsync(cancellationToken);
    }
} 