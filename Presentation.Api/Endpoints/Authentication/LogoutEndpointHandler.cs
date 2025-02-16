using Application.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Api.Endpoints.Authentication;

[AllowAnonymous]
[HttpPost("/auth/logout")]
public class LogoutEndpointHandler : EndpointWithoutRequest
{
    private readonly IAuthenticationService _authenticationService;
    
    public LogoutEndpointHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        String? refreshToken = _authenticationService.GetRefreshToken();

        if (refreshToken is not null)
        {
            await _authenticationService.RevokeUserSession(refreshToken);
        }
        
        _authenticationService.ClearTokenCookies();
        
        await SendOkAsync(cancellationToken);
    }
}