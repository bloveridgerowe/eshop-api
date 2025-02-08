using Application.DataTransferObjects;

namespace Application.Queries.Customers;

public class GetCustomerDetailsQueryResponse
{
    public CustomerDetails? CustomerDetails { get; init; }

    public static GetCustomerDetailsQueryResponse Empty => new();
}