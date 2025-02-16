using Application.DataTransferObjects;

namespace Presentation.Api.Endpoints.Categories;

public class GetCategoriesHttpResponse
{
    public List<CategoryDetails> Categories { get; init; } = [];
} 