using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.Commands.Customers;

public class UpdateCustomerDetailsCommandHandler : IRequestHandler<UpdateCustomerDetailsCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateCustomerDetailsCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCustomerDetailsCommand request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        Customer? customer = await _customerRepository.FindByIdAsync(request.CustomerId);
        
        if (customer == null)
        {
            throw new ArgumentException($"Customer with ID {request.CustomerId} not found.", nameof(request.CustomerId));
        }

        if (request.Name is not null)
        {
            customer.UpdateName(request.Name);
        }

        if (request.Address is not null)
        {
            customer.UpdateAddress(new Address(request.Address.FirstLine, request.Address.SecondLine, request.Address.City, request.Address.County, request.Address.PostCode));
        }

        if (request.CardDetails is not null)
        {
            customer.UpdateCardDetails(new CardDetails(request.CardDetails.CardNumber, request.CardDetails.ExpiryDate, request.CardDetails.Cvv));
        }

        await _customerRepository.SaveAsync(customer);
        await _unitOfWork.CommitAsync();
    }
    
    private static void ValidateRequest(UpdateCustomerDetailsCommand request)
    {
        if (request.CustomerId == Guid.Empty)
        {
            throw new ArgumentException("Customer ID cannot be empty", nameof(request.CustomerId));
        }
        
        if (request.Name is not null && String.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name cannot be empty if provided", nameof(request.Name));
        }
    }
} 