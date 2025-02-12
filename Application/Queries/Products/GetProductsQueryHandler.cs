using Application.Mappers;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Queries.Products;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, GetProductsQueryResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<GetProductsQueryResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        List<Product> products = [];
        
        if (request.Search is not null)
        {
            products = await _productRepository.FindByPartialNameAsync(request.Search);
        }
        else if (request.Category is not null)
        {
            products = await _productRepository.FindByCategoryAsync(request.Category.Value);
        }
        else if (request.Featured is not null)
        {
            products = await _productRepository.FindFeaturedAsync();
        }

        return new GetProductsQueryResponse
        {
            Products = products.Select(p => p.ToQueryModel()).ToList()
        };
    }
}