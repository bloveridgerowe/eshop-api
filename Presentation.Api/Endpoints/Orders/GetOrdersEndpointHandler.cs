using Application.Queries.Orders;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Orders;

[HttpGet("/me/orders")]
[Authorize]
public class GetOrdersEndpointHandler : EndpointWithoutRequest<GetOrdersHttpResponse>
{
    private readonly IMediator _mediator;

    public GetOrdersEndpointHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        GetOrdersQuery query = new GetOrdersQuery
        {
            CustomerId = User.GetId()
        };
        
        GetOrdersQueryResponse response = await _mediator.Send(query, cancellationToken);
        await SendOkAsync(response.ToHttpResponse(), cancellationToken);
    }
} 