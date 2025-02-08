using Application.DataTransferObjects;

namespace Application.Queries.Categories;

public class GetCategoriesQueryResponse
{
    public List<CategoryDetails> Categories { get; init; } = [];
} 