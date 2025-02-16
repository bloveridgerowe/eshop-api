using Application.DataTransferObjects;
using Domain.Entities;

namespace Application.Mappers;

public static class CustomerMapper
{
    public static CustomerDetails ToQueryModel(this Customer customer)
    {
        return new CustomerDetails
        {
            Name = customer.Name,
            Email = customer.Email,
            Address = customer.Address,
            CardDetails = customer.CardDetails
        };
    }
}