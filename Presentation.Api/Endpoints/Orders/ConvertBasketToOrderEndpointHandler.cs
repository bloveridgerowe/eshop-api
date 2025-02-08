using Application.Commands.Orders;
using Domain.Aggregates.Basket;
using Domain.Repositories;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Orders;

[HttpGet("/me/checkout")]
[Authorize]
public class ConvertBasketToOrderEndpointHandler : EndpointWithoutRequest<ConvertBasketToOrderHttpResponse>
{
    private readonly IMediator _mediator;
    private readonly IBasketRepository _basketRepository;

    public ConvertBasketToOrderEndpointHandler(IMediator mediator, IBasketRepository basketRepository)
    {
        _mediator = mediator;
        _basketRepository = basketRepository;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        Basket? basket = await _basketRepository.FindByCustomerIdAsync(User.GetId());

        if (basket is null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        ConvertBasketToOrderCommand command = new ConvertBasketToOrderCommand
        {
            CustomerId = User.GetId(),
            BasketId = basket.Id
        };

        ConvertBasketToOrderCommandResponse response = await _mediator.Send(command, cancellationToken);
        await SendOkAsync(response.ToHttpResponse(), cancellationToken);
    }
} 