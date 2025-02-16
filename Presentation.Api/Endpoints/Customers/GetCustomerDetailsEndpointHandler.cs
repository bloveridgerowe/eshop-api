using Application.Interfaces;
using Application.Queries.Customer;
using FastEndpoints;
using Infrastructure.Extensions;
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
    private readonly IAuthenticationService _authenticationService;

    public GetCustomerDetailsEndpointHandler(IMediator mediator, IAuthenticationService authenticationService)
    {
        _mediator = mediator;
        _authenticationService = authenticationService;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        _authenticationService.LogTokenCookies();
        
        GetCustomerDetailsQueryResponse customerDetailsQueryResponse = await _mediator.Send(new GetCustomerDetailsQuery
        {
            CustomerId = User.GetId() 
        });

        if (customerDetailsQueryResponse.CustomerDetails is null)
        {
            await SendNotFoundAsync();
            return;
        }
        
        await SendOkAsync(customerDetailsQueryResponse.ToHttpResponse());
    }
}