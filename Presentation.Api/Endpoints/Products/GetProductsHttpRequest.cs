using FastEndpoints;

namespace Presentation.Api.Endpoints.Products;

public class GetProductsHttpRequest
{
    [QueryParam] public Guid? Category { get; init; }
    [QueryParam] public Boolean? Featured { get; init; }
    [QueryParam] public String? Search { get; init; }
}