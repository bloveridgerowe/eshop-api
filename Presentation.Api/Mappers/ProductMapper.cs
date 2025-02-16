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

    public static GetProductHttpResponse ToHttpResponse(this GetProductQueryResponse response)
    {
        return new GetProductHttpResponse
        {
            ProductDetails = response.ProductDetails
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

    public static GetProductQuery ToQuery(this GetProductHttpRequest request)
    {
        return new GetProductQuery
        {
            ProductId = request.ProductId,
        };
    }
}