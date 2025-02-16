using Domain.Entities;
using Infrastructure.Persistence.Entities.Customer;
using Infrastructure.Persistence.Entities.Users;

namespace Infrastructure.Persistence.Mappers;

public static class CustomerMappers
{
    public static Customer ToDomain(this CustomerEntity entity)
    {
        return new Customer
        (
            id: entity.Id,
            name: entity.Name,
            address: entity.Address?.ToDomain(),
            email: entity.Email,
            cardDetails: entity.CardDetails?.ToDomain()
        );
    }

    public static CustomerEntity ToPersistence(this Customer domain)
    {
        return new CustomerEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Address = domain.Address?.ToPersistence(domain.Id),
            Email = domain.Email,
            CardDetails = domain.CardDetails?.ToPersistence(domain.Id)
        };
    }

    public static ApplicationUser ToUser(this Customer domain)
    {
        return new ApplicationUser
        {
            Id = domain.Id,
            Email = domain.Email,
            UserName = domain.Email
        };
    }
}