using Application.DataTransferObjects;
using Presentation.Api.Endpoints.Customers;

namespace Presentation.Api.Mappers;

public static class CustomerDetailsMapper
{
    public static SanitizedCustomerDetails ToSanitized(this CustomerDetails customerDetails)
    {
        return new SanitizedCustomerDetails
        {
            Address = customerDetails.Address,
            CardDetails = customerDetails.CardDetails?.ToSanitized(),
            Email = customerDetails.Email,
            Name = customerDetails.Name,
        };
    }
}