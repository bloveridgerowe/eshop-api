using Application.Queries.Products;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Products;

[AllowAnonymous]
[HttpGet("/products")]
public class GetProductsEndpointHandler : Endpoint<GetProductsHttpRequest, GetProductsHttpResponse>
{
    private readonly IMediator _mediator;

    public GetProductsEndpointHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(GetProductsHttpRequest request, CancellationToken cancellationToken)
    {
        GetProductsQueryResponse response = await _mediator.Send(request.ToQuery());
        
        await SendOkAsync(response.ToHttpResponse());
    }
}