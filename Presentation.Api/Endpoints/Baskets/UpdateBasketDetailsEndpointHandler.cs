using Application.Commands.Baskets;
using Domain.Repositories;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Baskets;

[HttpPatch("/me/basket")]
[Authorize]
public class UpdateBasketDetailsEndpointHandler : Endpoint<UpdateBasketDetailsHttpRequest>
{
    private readonly IMediator _mediator;

    public UpdateBasketDetailsEndpointHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(UpdateBasketDetailsHttpRequest httpRequest, CancellationToken cancellationToken)
    {
        await _mediator.Send(httpRequest.ToCommand(User));
        
        await SendOkAsync();
    }
} 