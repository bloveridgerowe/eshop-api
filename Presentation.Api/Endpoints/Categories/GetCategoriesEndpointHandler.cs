using Application.Queries.Categories;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Categories;

[AllowAnonymous]
[HttpGet("/categories")]
public class GetCategoriesEndpointHandler : EndpointWithoutRequest<GetCategoriesHttpResponse>
{
    private readonly IMediator _mediator;

    public GetCategoriesEndpointHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        GetCategoriesQueryResponse categoriesQueryResponse = await _mediator.Send(new GetCategoriesQuery());
        
        await SendOkAsync(categoriesQueryResponse.ToHttpResponse());
    }
} 