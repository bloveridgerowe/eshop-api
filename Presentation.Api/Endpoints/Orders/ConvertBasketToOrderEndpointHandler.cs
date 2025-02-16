using Application.Commands.Orders;
using Domain.Aggregates.Basket;
using Domain.Repositories;
using FastEndpoints;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Orders;

[HttpPost("/me/checkout")]
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
            await SendNotFoundAsync();
            return;
        }
        
        ConvertBasketToOrderCommandResponse response = await _mediator.Send(new ConvertBasketToOrderCommand
        {
            CustomerId = User.GetId(),
            BasketId = basket.Id
        });
        
        await SendOkAsync(response.ToHttpResponse());
    }
} 