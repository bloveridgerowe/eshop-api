using Application.DataTransferObjects;

namespace Application.Queries.Products;

public class GetProductsQueryResponse
{
    public List<ProductDetails> Products { get; init; }
}