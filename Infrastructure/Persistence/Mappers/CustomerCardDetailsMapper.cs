using Domain.ValueObjects;
using Infrastructure.Persistence.Entities.Customer;

namespace Infrastructure.Persistence.Mappers;

public static class CustomerCardDetailsMapper
{
    public static CustomerCardDetailsEntity ToPersistence(this CardDetails cardDetails, Guid customerId)
    {
        return new CustomerCardDetailsEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CardNumber = cardDetails.CardNumber,
            ExpiryDate = cardDetails.ExpiryDate,
            Cvv = cardDetails.Cvv
        };
    }

    public static CardDetails ToDomain(this CustomerCardDetailsEntity customerCardDetailsEntity)
    {
        return new CardDetails(customerCardDetailsEntity.CardNumber, customerCardDetailsEntity.ExpiryDate, customerCardDetailsEntity.Cvv);
    }
}