using Application.DataTransferObjects;
using Application.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Api.Endpoints.Authentication;

[AllowAnonymous]
[HttpPost("/token/refresh")]
public class RefreshEndpointHandler : EndpointWithoutRequest
{
    private readonly IAuthenticationService _authenticationService;
    
    public RefreshEndpointHandler(IAuthenticationService authenticationService, IHttpContextAccessor httpContextAccessor)
    {
        _authenticationService = authenticationService;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        String? refreshToken = _authenticationService.GetRefreshToken();

        if (String.IsNullOrEmpty(refreshToken))
        {
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        if (!await _authenticationService.ValidateRefreshToken(refreshToken))
        {
            _authenticationService.ClearTokenCookies();
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }
        
        TokenSet tokenSet = await _authenticationService.CreateTokensForUser(refreshToken);
        _authenticationService.SetTokenCookies(tokenSet);
        await SendOkAsync(cancellationToken);
    }
}