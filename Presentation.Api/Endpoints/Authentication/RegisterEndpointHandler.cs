using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Api.Endpoints.Authentication;

[AllowAnonymous]
[HttpPost("/auth/register")]
public class RegisterEndpointHandler : Endpoint<RegisterHttpRequest>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public RegisterEndpointHandler(IAuthenticationService authenticationService, ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _authenticationService = authenticationService;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task HandleAsync(RegisterHttpRequest request, CancellationToken cancellationToken)
    {
        Guid userId = await _authenticationService.RegisterUser(request.Email, request.Password);
        
        Customer customer = new Customer(userId, request.Email);
        await _customerRepository.SaveAsync(customer);
        await _unitOfWork.CommitAsync();

        await SendOkAsync(customer);
    }
}