using Application.DataTransferObjects;

namespace Application.Queries.Customer;

public class GetCustomerDetailsQueryResponse
{
    public CustomerDetails? CustomerDetails { get; init; }

    public static GetCustomerDetailsQueryResponse Empty => new();
}