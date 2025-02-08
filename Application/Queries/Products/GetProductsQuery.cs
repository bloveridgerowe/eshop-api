using MediatR;

namespace Application.Queries.Products;

public class GetProductsQuery : IRequest<GetProductsQueryResponse>
{
    public Guid? Category { get; init; }
    public String? Search { get; init; }
    public Boolean? Featured { get; init; }
}