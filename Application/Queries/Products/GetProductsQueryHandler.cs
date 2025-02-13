using Application.DataTransferObjects;
using Application.Services;
using MediatR;

namespace Application.Queries.Products;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, GetProductsQueryResponse>
{
    private readonly IProductQueryService _productQueryService;

    public GetProductsQueryHandler(IProductQueryService productQueryService)
    {
        _productQueryService = productQueryService;
    }
    
    public async Task<GetProductsQueryResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        List<ProductSummary> products = [];
        
        if (request.Search is not null)
        {
            products = await _productQueryService.FindByPartialNameAsync(request.Search);
        }
        else if (request.Category is not null)
        {
            products = await _productQueryService.FindByCategoryAsync(request.Category.Value);
        }
        else if (request.Featured is not null)
        {
            products = await _productQueryService.FindFeaturedAsync();
        }

        return new GetProductsQueryResponse
        {
            Products = products
        };
    }
}