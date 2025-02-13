using Application.Mappers;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Queries.Products;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, GetProductQueryResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<GetProductQueryResponse> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.FindByIdAsync(request.ProductId);

        if (product is null)
        {
            return GetProductQueryResponse.Empty;
        }

        return new GetProductQueryResponse
        {
            ProductDetails = product.ToDetailsQueryModel()
        };
    }
}