using Application.DataTransferObjects;

namespace Presentation.Api.Endpoints.Products;

public class GetProductsHttpResponse
{
    public List<ProductDetails> Products { get; init; } = [];
}