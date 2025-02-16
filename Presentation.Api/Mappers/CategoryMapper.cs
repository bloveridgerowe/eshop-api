using Application.Queries.Categories;
using Presentation.Api.Endpoints.Categories;

namespace Presentation.Api.Mappers;

public static class CategoryMapper
{
    public static GetCategoriesHttpResponse ToHttpResponse(this GetCategoriesQueryResponse response)
    {
        return new GetCategoriesHttpResponse
        {
            Categories = response.Categories
        };
    }
}