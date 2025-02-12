using Application.Queries.Customer;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Customers;

[HttpGet("/me")]
[Authorize]
public class GetCustomerDetailsEndpointHandler : EndpointWithoutRequest<GetCustomerDetailsHttpResponse>
{
    private readonly IMediator _mediator;

    public GetCustomerDetailsEndpointHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        GetCustomerDetailsQuery query = new GetCustomerDetailsQuery
        {
            CustomerId = User.GetId()
        };
        
        GetCustomerDetailsQueryResponse customerDetailsQueryResponse = await _mediator.Send(query, cancellationToken);

        if (customerDetailsQueryResponse.CustomerDetails is null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }
        
        await SendOkAsync(customerDetailsQueryResponse.ToHttpResponse(), cancellationToken);
    }
}