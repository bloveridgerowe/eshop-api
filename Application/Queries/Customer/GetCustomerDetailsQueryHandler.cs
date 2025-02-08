using Application.Mappers;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Queries.Customers;

public class GetCustomerDetailsQueryHandler : IRequestHandler<GetCustomerDetailsQuery, GetCustomerDetailsQueryResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerDetailsQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<GetCustomerDetailsQueryResponse> Handle(GetCustomerDetailsQuery request, CancellationToken cancellationToken)
    {
        Customer? customer = await _customerRepository.FindByIdAsync(request.CustomerId);

        if (customer == null)
        {
            return GetCustomerDetailsQueryResponse.Empty;
        }

        return new GetCustomerDetailsQueryResponse
        {
            CustomerDetails = customer.ToCommandQueryModel()
        };
    }
}