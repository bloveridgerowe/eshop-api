using Domain.ValueObjects;
using Presentation.Api.Endpoints.Customers;

namespace Presentation.Api.Mappers;

public static class CardDetailsMapper
{
    public static SanitizedCardDetails ToSanitized(this CardDetails cardDetails)
    {
        return new SanitizedCardDetails
        {
            CardNumber = cardDetails.MaskedCardNumber,
            ExpiryDate = cardDetails.MaskedExpiryDate,
            Cvv = cardDetails.MaskedCvv
        };
    }
}