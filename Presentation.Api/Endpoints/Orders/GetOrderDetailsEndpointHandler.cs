using Application.Queries.Orders;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Orders;

[HttpGet("/me/orders/{orderId}")]
[Authorize]
public class GetOrderDetailsEndpointHandler : Endpoint<GetOrderDetailsHttpRequest, GetOrderDetailsHttpResponse>
{
    private readonly IMediator _mediator;

    public GetOrderDetailsEndpointHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(GetOrderDetailsHttpRequest request, CancellationToken cancellationToken)
    {
        GetOrderDetailsQueryResponse response = await _mediator.Send(request.ToQuery(User));

        if (response.OrderDetails is null)
        {
            await SendNotFoundAsync();
            return;
        }

        await SendOkAsync(response.ToHttpResponse());
    }
} 