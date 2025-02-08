using Application.Commands.Baskets;
using Domain.Repositories;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;

namespace Presentation.Api.Endpoints.Baskets;

[HttpPost("/me/basket")]
[Authorize]
public class UpdateBasketDetailsEndpointHandler : Endpoint<UpdateBasketDetailsHttpRequest>
{
    private readonly IMediator _mediator;
    private readonly IBasketRepository _basketRepository;

    public UpdateBasketDetailsEndpointHandler(IMediator mediator, IBasketRepository basketRepository)
    {
        _mediator = mediator;
        _basketRepository = basketRepository;
    }

    public override async Task HandleAsync(UpdateBasketDetailsHttpRequest httpRequest, CancellationToken cancellationToken)
    {
        UpdateBasketDetailsCommand command = new UpdateBasketDetailsCommand
        {
            CustomerId = User.GetId(),
            Items = httpRequest.Items
        };

        await _mediator.Send(command, cancellationToken);
        await SendOkAsync(cancellationToken);
    }
} 