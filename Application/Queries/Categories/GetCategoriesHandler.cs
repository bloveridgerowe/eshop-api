using Application.Mappers;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Queries.Categories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, GetCategoriesQueryResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<GetCategoriesQueryResponse> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        List<Category> categories = await _categoryRepository.GetAllCategoriesAsync();
        
        GetCategoriesQueryResponse response = new GetCategoriesQueryResponse
        {
            Categories = categories.Select(category => category.ToCommandQueryModel()).ToList()
        };

        return response;
    }
} 