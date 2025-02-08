using Application.Queries.Products;
using Presentation.Api.Endpoints.Products;

namespace Presentation.Api.Mappers;

public static class ProductMapper
{
    public static GetProductsHttpResponse ToHttpResponse(this GetProductsQueryResponse response)
    {
        return new GetProductsHttpResponse
        {
            Products = response.Products
        };
    }

    public static GetProductsQuery ToQuery(this GetProductsHttpRequest request)
    {
        return new GetProductsQuery
        {
            Search = request.Search,
            Category = request.Category,
            Featured = request.Featured
        };
    }
}