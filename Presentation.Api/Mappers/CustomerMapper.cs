using System.Security.Claims;
using Application.Commands.Baskets;
using Application.Commands.Customers;
using Application.Queries.Customer;
using Infrastructure.Extensions;
using Presentation.Api.Endpoints.Customers;
using Presentation.Api.Extensions;

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

    public static UpdateCustomerDetailsCommand ToCommand(this UpdateCustomerDetailsHttpRequest request, ClaimsPrincipal user)
    {
        return new UpdateCustomerDetailsCommand
        {
            CustomerId = user.GetId(),
            Address = request.Address,
            CardDetails = request.CardDetails,
            Name = request.Name
        };
    }
}