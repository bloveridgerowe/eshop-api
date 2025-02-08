using Domain.ValueObjects;
using Infrastructure.Persistence.Entities.Customer;

namespace Infrastructure.Persistence.Mappers;

public static class AddressMapper
{
    public static Address ToDomain(this CustomerAddressEntity entity)
    {
        return new Address(entity.FirstLine, entity.SecondLine, entity.City, entity.County, entity.PostCode);
    }

    public static CustomerAddressEntity ToPersistence(this Address domain, Guid customerId)
    {
        return new CustomerAddressEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            FirstLine = domain.FirstLine,
            SecondLine = domain.SecondLine,
            City = domain.City,
            County = domain.County,
            PostCode = domain.PostCode
        };
    }
}