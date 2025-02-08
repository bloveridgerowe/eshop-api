using Application.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Api.Endpoints.Authentication;

[Authorize]
[HttpPost("/auth/change-password")]
public class ChangePasswordEndpointHandler : Endpoint<ChangePasswordHttpRequest>
{
    private readonly IAuthenticationService _authenticationService;
    
    public ChangePasswordEndpointHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public override async Task HandleAsync(ChangePasswordHttpRequest request, CancellationToken cancellationToken)
    {
        await _authenticationService.ChangeUserPassword(request.OldPassword, request.NewPassword);

        await SendOkAsync(cancellationToken);
    }
}