using Application.DataTransferObjects;

namespace Application.Queries.Products;

public class GetProductsQueryResponse
{
    public List<ProductSummary> Products { get; init; }
}