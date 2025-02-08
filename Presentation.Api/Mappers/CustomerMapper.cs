using Application.Queries.Customers;
using Presentation.Api.Endpoints.Customers;

namespace Presentation.Api.Mappers;

public static class CustomerMapper
{
    public static GetCustomerDetailsHttpResponse ToHttpResponse(this GetCustomerDetailsQueryResponse query)
    {
        return new GetCustomerDetailsHttpResponse
        {
            CustomerDetails = query.CustomerDetails?.ToSanitized(),
        };
    }
}