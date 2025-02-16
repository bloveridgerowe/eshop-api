using Application.DataTransferObjects;
using Application.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Api.Endpoints.Authentication;

[AllowAnonymous]
[HttpPost("/auth/login")]
public class LoginEndpointHandler : Endpoint<LoginHttpRequest>
{
    private readonly IAuthenticationService _authenticationService;
    
    public LoginEndpointHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public override async Task HandleAsync(LoginHttpRequest request, CancellationToken cancellationToken)
    {
        Boolean userValid = await _authenticationService.ValidateUser(request.Email, request.Password);
        
        if (!userValid)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        TokenSet tokenSet = await _authenticationService.CreateTokensForUser(request.Email, request.Password);
        _authenticationService.SetTokenCookies(tokenSet);
        
        await SendOkAsync(cancellationToken);
    }
}