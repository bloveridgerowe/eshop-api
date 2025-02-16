using Application.DataTransferObjects;
using Application.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Api.Endpoints.Authentication;

[AllowAnonymous]
[HttpPost("/auth/login/demo")]
public class DemoLoginEndpointHandler : EndpointWithoutRequest
{
    private readonly IAuthenticationService _authenticationService;
    
    public DemoLoginEndpointHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        String demoEmail = "tim.murphy@eshop.com";
        String demoPassword = "Password123!";
        
        Boolean userValid = await _authenticationService.ValidateUser(demoEmail, demoPassword);
        
        if (!userValid)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        TokenSet tokenSet = await _authenticationService.CreateTokensForUser(demoEmail, demoPassword);
        _authenticationService.SetTokenCookies(tokenSet);
        
        await SendOkAsync(cancellationToken);
    }
}