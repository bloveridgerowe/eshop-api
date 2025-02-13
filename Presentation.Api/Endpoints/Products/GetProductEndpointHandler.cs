using Application.Queries.Products;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Products;

[AllowAnonymous]
[HttpGet("/products/{productId}")]
public class GetProductEndpointHandler  : Endpoint<GetProductHttpRequest, GetProductHttpResponse>
{
    private readonly IMediator _mediator;

    public GetProductEndpointHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(GetProductHttpRequest request, CancellationToken cancellationToken)
    {
        GetProductQueryResponse response = await _mediator.Send(request.ToQuery());

        if (response.ProductDetails is null)
        {
            await SendNotFoundAsync();
            return;
        }
        
        await SendOkAsync(response.ToHttpResponse());
    }
}