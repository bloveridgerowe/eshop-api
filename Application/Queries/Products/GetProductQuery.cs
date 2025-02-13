using MediatR;

namespace Application.Queries.Products;

public class GetProductQuery : IRequest<GetProductQueryResponse>
{
    public required Guid ProductId { get; init; }
}