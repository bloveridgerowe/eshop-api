using Application.Commands.Customers;
using MediatR;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Presentation.Api.Extensions;

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
        UpdateCustomerDetailsCommand updateCustomerDetailsCommand = new UpdateCustomerDetailsCommand
        {
            CustomerId = User.GetId(),
            Address = request.Address,
            CardDetails = request.CardDetails,
            Name = request.Name
        };
        
        await _mediator.Send(updateCustomerDetailsCommand, cancellationToken);

        await SendOkAsync(cancellationToken);
    }
}