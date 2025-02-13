using Application.DataTransferObjects;

namespace Presentation.Api.Endpoints.Products;

public class GetProductsHttpResponse
{
    public List<ProductSummary> Products { get; init; } = [];
}