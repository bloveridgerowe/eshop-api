using Application.DataTransferObjects;

namespace Presentation.Api.Endpoints.Products;

public class GetProductHttpResponse
{
    public required ProductDetails? ProductDetails { get; init; }
}