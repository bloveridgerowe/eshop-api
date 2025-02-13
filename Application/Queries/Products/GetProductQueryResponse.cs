using Application.DataTransferObjects;

namespace Application.Queries.Products;

public class GetProductQueryResponse
{
    public ProductDetails? ProductDetails { get; init; }

    public static GetProductQueryResponse Empty = new();
}