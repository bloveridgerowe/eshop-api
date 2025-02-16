using Application.Commands.Customers;
using MediatR;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;
using Presentation.Api.Mappers;

namespace Presentation.Api.Endpoints.Customers;

[HttpPost("/me")]
[Authorize]
public class UpdateCustomerDetailsEndpointHandler : Endpoint<UpdateCustomerDetailsHttpRequest>
{
    private readonly IMediator _mediator;

    public UpdateCustomerDetailsEndpointHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(UpdateCustomerDetailsHttpRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request.ToCommand(User));
        
        await SendOkAsync();
    }
}